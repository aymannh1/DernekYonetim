using DernekYonetim.Models;
using DernekYonetim.Services;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DernekYonetim.Controllers
{
    [Authorize]
    public class AidatController : Controller
    {
        private readonly AidatService _aidatService;
        private readonly UserManager<Uye> _userManager;
        private readonly IWebHostEnvironment _env;

        public AidatController(
            AidatService aidatService,
            UserManager<Uye> userManager,
            IWebHostEnvironment env)
        {
            _aidatService = aidatService;
            _userManager = userManager;
            _env = env;
        }

        // ── GET /Aidat/Index (Admin) ──────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Index(string? donem, OdemeDurumu? durum)
        {
            var liste = await _aidatService.ListeleAsync(donem, durum);
            var istatistik = await _aidatService.IstatistikAsync(donem);
            var donemler = await _aidatService.DonemListesiAsync();

            ViewBag.Donem = donem;
            ViewBag.Durum = durum;
            ViewBag.Donemler = donemler;
            ViewBag.Istatistik = istatistik;

            return View(liste);
        }

        // ── GET /Aidat/TopluOlustur ───────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public IActionResult TopluOlustur()
        {
            return View(new TopluAidatViewModel());
        }

        // ── POST /Aidat/TopluOlustur ──────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> TopluOlustur(TopluAidatViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var sayac = await _aidatService.TopluAidatOlusturAsync(
                model.Donem, model.Tutar, model.SonOdemeTarihi);

            TempData["Basari"] = $"{sayac} üye için aidat kaydı oluşturuldu.";
            return RedirectToAction("Index");
        }

        // ── POST /Aidat/DekontOnayla ──────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> DekontOnayla(int id)
        {
            var adminId = _userManager.GetUserId(User)!;
            await _aidatService.DekontOnaylaAsync(id, adminId);
            TempData["Basari"] = "Dekont onaylandı, aidat ödendi olarak işaretlendi.";
            return RedirectToAction("Index");
        }

        // ── GET /Aidat/Aidatlarim (Üye) ──────────────────────────────
        public async Task<IActionResult> Aidatlarim()
        {
            var uyeId = _userManager.GetUserId(User)!;
            var liste = await _aidatService.UyeAidatlariAsync(uyeId);
            return View(liste);
        }

        // ── GET /Aidat/DekontYukle/5 ─────────────────────────────────
        public async Task<IActionResult> DekontYukle(int id)
        {
            var uyeId = _userManager.GetUserId(User)!;
            var aidatlar = await _aidatService.UyeAidatlariAsync(uyeId);
            var aidat = aidatlar.FirstOrDefault(a => a.Id == id);

            if (aidat == null) return NotFound();

            return View(new DekontYukleViewModel
            {
                AidatId = aidat.Id,
                Donem = aidat.Donem,
                Tutar = aidat.Tutar
            });
        }

        // ── POST /Aidat/DekontYukle ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DekontYukle(DekontYukleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var uyeId = _userManager.GetUserId(User)!;

            // Dosyayı kaydet
            string dekontUrl = string.Empty;
            if (model.Dekont != null && model.Dekont.Length > 0)
            {
                var klasor = Path.Combine(_env.WebRootPath, "uploads", "dekontlar");
                Directory.CreateDirectory(klasor);
                var dosyaAdi = $"{uyeId}_{model.AidatId}_{Path.GetFileName(model.Dekont.FileName)}";
                var yol = Path.Combine(klasor, dosyaAdi);

                using var stream = new FileStream(yol, FileMode.Create);
                await model.Dekont.CopyToAsync(stream);

                dekontUrl = $"/uploads/dekontlar/{dosyaAdi}";
            }

            await _aidatService.DekontYukleAsync(
                model.AidatId, uyeId, dekontUrl, model.OdemeYontemi);

            TempData["Basari"] = "Dekontunuz yüklendi, onay bekleniyor.";
            return RedirectToAction("Aidatlarim");
        }
    }
}