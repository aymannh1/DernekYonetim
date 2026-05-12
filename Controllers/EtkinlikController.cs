using DernekYonetim.Models;
using DernekYonetim.Services;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DernekYonetim.Controllers
{
    [Authorize]
    public class EtkinlikController : Controller
    {
        private readonly EtkinlikService _etkinlikService;
        private readonly UserManager<Uye> _userManager;

        public EtkinlikController(EtkinlikService etkinlikService, UserManager<Uye> userManager)
        {
            _etkinlikService = etkinlikService;
            _userManager = userManager;
        }

        // ── GET /Etkinlik/Index ───────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var etkinlikler = await _etkinlikService.ListeleAsync(sadeceyayinda: true);
            return View(etkinlikler);
        }

        // ── GET /Etkinlik/Detay/5 ─────────────────────────────────────
        public async Task<IActionResult> Detay(int id)
        {
            var etkinlik = await _etkinlikService.BulAsync(id);
            if (etkinlik == null) return NotFound();

            var uyeId = _userManager.GetUserId(User);
            ViewBag.KayitliMi = etkinlik.Kayitlar
                .Any(k => k.UyeId == uyeId && k.Durum == EtkinlikKayitDurumu.Onaylandi);

            return View(etkinlik);
        }

        // ── POST /Etkinlik/KayitOl ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KayitOl(int id)
        {
            var uyeId = _userManager.GetUserId(User)!;
            var (basarili, mesaj) = await _etkinlikService.KayitOlAsync(id, uyeId);

            if (basarili)
                TempData["Basari"] = mesaj;
            else
                TempData["Hata"] = mesaj;

            return RedirectToAction("Detay", new { id });
        }

        // ── POST /Etkinlik/KayitIptal ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KayitIptal(int id)
        {
            var uyeId = _userManager.GetUserId(User)!;
            await _etkinlikService.KayitIptalAsync(id, uyeId);
            TempData["Basari"] = "Kayıt iptal edildi.";
            return RedirectToAction("Detay", new { id });
        }

        // ── GET /Etkinlik/Olustur (Admin) ─────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public IActionResult Olustur()
        {
            return View(new EtkinlikViewModel());
        }

        // ── POST /Etkinlik/Olustur ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Olustur(EtkinlikViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var etkinlik = new Etkinlik
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Kategori = model.Kategori,
                Yer = model.Yer,
                BaslangicTarihi = model.BaslangicTarihi,
                BitisTarihi = model.BitisTarihi,
                Kontenjan = model.Kontenjan,
                UcretsizMi = model.UcretsizMi,
                Ucret = model.UcretsizMi ? null : model.Ucret,
                Durum = model.Durum,
                OlusturanId = _userManager.GetUserId(User)!,
                OlusturmaTarihi = DateTime.UtcNow
            };

            await _etkinlikService.OlusturAsync(etkinlik);
            TempData["Basari"] = "Etkinlik oluşturuldu.";
            return RedirectToAction("Index");
        }

        // ── GET /Etkinlik/Duzenle/5 ───────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var etkinlik = await _etkinlikService.BulAsync(id);
            if (etkinlik == null) return NotFound();

            var model = new EtkinlikViewModel
            {
                Id = etkinlik.Id,
                Baslik = etkinlik.Baslik,
                Aciklama = etkinlik.Aciklama,
                Kategori = etkinlik.Kategori,
                Yer = etkinlik.Yer,
                BaslangicTarihi = etkinlik.BaslangicTarihi,
                BitisTarihi = etkinlik.BitisTarihi,
                Kontenjan = etkinlik.Kontenjan,
                UcretsizMi = etkinlik.UcretsizMi,
                Ucret = etkinlik.Ucret,
                Durum = etkinlik.Durum
            };
            return View(model);
        }

        // ── POST /Etkinlik/Duzenle ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(EtkinlikViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var etkinlik = await _etkinlikService.BulAsync(model.Id);
            if (etkinlik == null) return NotFound();

            etkinlik.Baslik = model.Baslik;
            etkinlik.Aciklama = model.Aciklama;
            etkinlik.Kategori = model.Kategori;
            etkinlik.Yer = model.Yer;
            etkinlik.BaslangicTarihi = model.BaslangicTarihi;
            etkinlik.BitisTarihi = model.BitisTarihi;
            etkinlik.Kontenjan = model.Kontenjan;
            etkinlik.UcretsizMi = model.UcretsizMi;
            etkinlik.Ucret = model.UcretsizMi ? null : model.Ucret;
            etkinlik.Durum = model.Durum;

            await _etkinlikService.GuncelleAsync(etkinlik);
            TempData["Basari"] = "Etkinlik güncellendi.";
            return RedirectToAction("Index");
        }

        // ── POST /Etkinlik/Sil ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Sil(int id)
        {
            await _etkinlikService.SilAsync(id);
            TempData["Basari"] = "Etkinlik silindi.";
            return RedirectToAction("Index");
        }

        // ── GET /Etkinlik/Katilimcilar/5 (Admin) ─────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Katilimcilar(int id)
        {
            var etkinlik = await _etkinlikService.BulAsync(id);
            if (etkinlik == null) return NotFound();
            return View(etkinlik);
        }
    }
}