using DernekYonetim.Models;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DernekYonetim.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<Uye> _signInManager;
        private readonly UserManager<Uye> _userManager;
        private readonly DernekYonetim.Data.AppDbContext _db;

        public AuthController(
            SignInManager<Uye> signInManager,
            UserManager<Uye> userManager,
            DernekYonetim.Data.AppDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        // ── GET /Auth/Login ───────────────────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // ── POST /Auth/Login ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Sifre,
                model.BeniHatirla,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var uye = await _userManager.FindByEmailAsync(model.Email);
                if (uye != null)
                {
                    uye.SonGiris = DateTime.UtcNow;
                    await _userManager.UpdateAsync(uye);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError("", "Hesabınız kilitlendi. 15 dakika sonra tekrar deneyin.");
            else
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");

            return View(model);
        }

        // ── GET /Auth/Logout ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ── GET /Auth/Basvuru ─────────────────────────────────────────
        [HttpGet]
        public IActionResult Basvuru()
        {
            return View();
        }

        // ── POST /Auth/Basvuru ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Basvuru(BasvuruViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Aynı e-posta ile başvuru var mı?
            var mevcutBasvuru = _db.Basvurular
                .Any(b => b.Email == model.Email);
            var mevcutUye = await _userManager.FindByEmailAsync(model.Email);

            if (mevcutBasvuru || mevcutUye != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta ile zaten bir başvuru veya hesap mevcut.");
                return View(model);
            }

            var basvuru = new DernekYonetim.Models.Basvuru
            {
                Ad = model.Ad,
                Soyad = model.Soyad,
                Email = model.Email,
                Telefon = model.Telefon,
                Adres = model.Adres,
                Meslek = model.Meslek,
                BasvuruNotu = model.BasvuruNotu,
                BasvuruTarihi = DateTime.UtcNow
            };

            _db.Basvurular.Add(basvuru);
            await _db.SaveChangesAsync();

            TempData["Mesaj"] = "Başvurunuz alındı! Yönetici onayından sonra e-posta ile bilgilendirileceksiniz.";
            return RedirectToAction("BasvuruTamam");
        }

        // ── GET /Auth/BasvuruTamam ────────────────────────────────────
        [HttpGet]
        public IActionResult BasvuruTamam()
        {
            return View();
        }

        // ── GET /Auth/AccessDenied ────────────────────────────────────
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}