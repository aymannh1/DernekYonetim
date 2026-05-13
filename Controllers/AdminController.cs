using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    [Authorize(Roles = "Admin,Yonetim")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;
        private readonly UyeService _uyeService;
        private readonly EmailService _emailService;

        public AdminController(
            AppDbContext db,
            UserManager<Uye> userManager,
            UyeService uyeService,
            EmailService emailService)
        {
            _db = db;
            _userManager = userManager;
            _uyeService = uyeService;
            _emailService = emailService;
        }

        // ── GET /Admin/Index ──────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var istatistik = await _uyeService.IstatistikGetirAsync();
            return View(istatistik);
        }

        // ── GET /Admin/Basvurular ─────────────────────────────────────
        public async Task<IActionResult> Basvurular()
        {
            var liste = await _db.Basvurular
                .OrderByDescending(b => b.BasvuruTarihi)
                .ToListAsync();
            return View(liste);
        }

        // ── POST /Admin/BasvuruOnayla ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BasvuruOnayla(int id)
        {
            var basvuru = await _db.Basvurular.FindAsync(id);
            if (basvuru == null)
            {
                TempData["Hata"] = "Başvuru bulunamadı.";
                return RedirectToAction("Basvurular");
            }

            var adminId = _userManager.GetUserId(User)!;
            var sonuc = await _uyeService.BasvuruOnaylaAsync(id, adminId);

            if (sonuc.Succeeded)
            {
                TempData["Basari"] = "Başvuru onaylandı, üye hesabı oluşturuldu.";

                var geciciSifre = $"Dernek@{DateTime.UtcNow.Year}!";
                var adSoyad = $"{basvuru.Ad} {basvuru.Soyad}";
                var icerik = $@"
<div style='font-family:sans-serif;max-width:560px;margin:auto'>
  <h2 style='color:#1B6B40'>HABSİS – Üyeliğiniz Onaylandı</h2>
  <p>Sayın <strong>{adSoyad}</strong>,</p>
  <p>HAKİD – Habesistan Kalkınma ve İşbirliği Derneği üyelik başvurunuz onaylanmıştır.</p>
  <p>Sisteme aşağıdaki bilgilerle giriş yapabilirsiniz:</p>
  <table style='border-collapse:collapse;width:100%'>
    <tr><td style='padding:8px;font-weight:bold'>E-posta</td><td style='padding:8px'>{basvuru.Email}</td></tr>
    <tr style='background:#f5f5f5'><td style='padding:8px;font-weight:bold'>Geçici Şifre</td><td style='padding:8px'><code>{geciciSifre}</code></td></tr>
  </table>
  <p style='margin-top:16px;color:#666'>İlk girişinizden sonra şifrenizi değiştirmenizi öneririz.</p>
  <a href='https://habsis.onrender.com/Auth/Login'
     style='display:inline-block;margin-top:12px;padding:10px 24px;background:#1B6B40;color:#fff;text-decoration:none;border-radius:6px'>
    Giriş Yap
  </a>
</div>";

                await _emailService.GonderAsync(basvuru.Email, adSoyad,
                    "HABSİS – Üyeliğiniz Onaylandı", icerik);
            }
            else
            {
                TempData["Hata"] = string.Join(", ", sonuc.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Basvurular");
        }

        // ── POST /Admin/BasvuruReddet ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BasvuruReddet(int id, string redNedeni)
        {
            var basvuru = await _db.Basvurular.FindAsync(id);
            if (basvuru != null)
            {
                basvuru.Durum = BasvuruDurumu.Reddedildi;
                basvuru.RedNedeni = redNedeni;
                basvuru.IslemTarihi = DateTime.UtcNow;
                basvuru.IslemYapanId = _userManager.GetUserId(User);
                await _db.SaveChangesAsync();
                TempData["Basari"] = "Başvuru reddedildi.";
            }
            return RedirectToAction("Basvurular");
        }

        // ── GET /Admin/Uyeler ─────────────────────────────────────────
        public async Task<IActionResult> Uyeler(string? arama, int sayfa = 1)
        {
            var (liste, toplamSayfa) = await _uyeService.ListeleAsync(arama, null, sayfa);
            ViewBag.Arama = arama;
            ViewBag.Sayfa = sayfa;
            ViewBag.ToplamSayfa = toplamSayfa;
            return View(liste);
        }

        // ── GET /Admin/UyeDetay ───────────────────────────────────────
        public async Task<IActionResult> UyeDetay(string id)
        {
            var uye = await _uyeService.BulAsync(id);
            if (uye == null) return NotFound();
            return View(uye);
        }

        // ── POST /Admin/DurumGuncelle ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DurumGuncelle(string id, UyelikDurumu durum)
        {
            await _uyeService.DurumGuncelleAsync(id, durum);
            TempData["Basari"] = "Üye durumu güncellendi.";
            return RedirectToAction("UyeDetay", new { id });
        }
        // ── GET /Admin/TopluMail ──────────────────────────────────────
        [HttpGet]
        public IActionResult TopluMail()
        {
            return View(new DernekYonetim.ViewModels.TopluMailViewModel());
        }

        // ── POST /Admin/TopluMail ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopluMail(DernekYonetim.ViewModels.TopluMailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Hedef üyeleri belirle
            var q = _db.Users.AsQueryable();

            if (model.Hedef == "Aktif")
                q = q.Where(u => u.UyelikDurumu == Models.UyelikDurumu.Aktif);
            else if (model.Hedef == "Gecikti")
            {
                var gecikmiUyeIdler = _db.Aidatlar
                    .Where(a => a.OdemeDurumu == Models.OdemeDurumu.Gecikti)
                    .Select(a => a.UyeId)
                    .Distinct();
                q = q.Where(u => gecikmiUyeIdler.Contains(u.Id));
            }

            var uyeler = await q
                .Where(u => u.Email != null)
                .Select(u => new { u.Email, u.Ad, u.Soyad })
                .ToListAsync();

            var alicilar = uyeler
                .Select(u => (u.Email!, $"{u.Ad} {u.Soyad}"))
                .ToList();

            var (basarili, basarisiz) = await _emailService.TopluGonderAsync(
                alicilar, model.Konu, model.Icerik);

            TempData["Basari"] = $"Mail gönderildi: {basarili} başarılı, {basarisiz} başarısız.";
            return RedirectToAction("TopluMail");
        }
    }
}