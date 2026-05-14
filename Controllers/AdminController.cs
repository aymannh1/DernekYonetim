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
        private readonly BildirimService _bildirimService;

        public AdminController(
            AppDbContext db,
            UserManager<Uye> userManager,
            UyeService uyeService,
            BildirimService bildirimService)
        {
            _db = db;
            _userManager = userManager;
            _uyeService = uyeService;
            _bildirimService = bildirimService;
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
                var yeniUye = await _userManager.FindByEmailAsync(basvuru.Email);
                if (yeniUye != null)
                {
                    await _bildirimService.EkleAsync(
                        yeniUye.Id,
                        "Üyeliğiniz Onaylandı",
                        $"Sayın {basvuru.Ad} {basvuru.Soyad}, HAKİD üyelik başvurunuz onaylandı. " +
                        $"Sisteme e-posta adresiniz ve geçici şifreniz '{geciciSifre}' ile giriş yapabilirsiniz. " +
                        "İlk girişinizden sonra şifrenizi değiştirmenizi öneririz.",
                        BildirimTuru.Basvuru,
                        "/Auth/Login");
                }
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

            var uyeIdler = await q.Select(u => u.Id).ToListAsync();

            var (basarili, _) = await _bildirimService.TopluEkleAsync(
                uyeIdler, model.Konu, model.Icerik, BildirimTuru.Genel);

            if (basarili > 0)
                TempData["Basari"] = $"Mesaj gönderildi: {basarili} üyeye bildirim oluşturuldu.";
            else
                TempData["Hata"] = "Seçilen grupta üye bulunamadı.";

            return RedirectToAction("TopluMail");
        }
    }
}