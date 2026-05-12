using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    [Authorize]
    public class UyeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;

        public UyeController(AppDbContext db, UserManager<Uye> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET /Uye/Index ────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var uye = await _db.Users
                .Include(u => u.Aidatlar)
                .Include(u => u.EtkinlikKayitlari)
                    .ThenInclude(ek => ek.Etkinlik)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (uye == null) return NotFound();
            return View(uye);
        }

        // ── GET /Uye/Profil ───────────────────────────────────────────
        public async Task<IActionResult> Profil()
        {
            var uye = await _userManager.GetUserAsync(User);
            if (uye == null) return NotFound();

            var model = new ProfilViewModel
            {
                Ad = uye.Ad,
                Soyad = uye.Soyad,
                Telefon = uye.Telefon,
                Meslek = uye.Meslek,
                Adres = uye.Adres
            };
            return View(model);
        }

        // ── POST /Uye/Profil ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(ProfilViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var uye = await _userManager.GetUserAsync(User);
            if (uye == null) return NotFound();

            uye.Ad = model.Ad;
            uye.Soyad = model.Soyad;
            uye.Telefon = model.Telefon;
            uye.Meslek = model.Meslek;
            uye.Adres = model.Adres;

            await _userManager.UpdateAsync(uye);
            TempData["Basari"] = "Profiliniz güncellendi.";
            return RedirectToAction("Profil");
        }

        // ── GET /Uye/SifreDegistir ────────────────────────────────────
        public IActionResult SifreDegistir()
        {
            return View();
        }

        // ── POST /Uye/SifreDegistir ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir(SifreDegistirViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var uye = await _userManager.GetUserAsync(User);
            if (uye == null) return NotFound();

            var sonuc = await _userManager.ChangePasswordAsync(
                uye, model.MevcutSifre, model.YeniSifre);

            if (sonuc.Succeeded)
            {
                TempData["Basari"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("Profil");
            }

            foreach (var hata in sonuc.Errors)
                ModelState.AddModelError("", hata.Description);

            return View(model);
        }
    }
}