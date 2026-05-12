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
    public class BelgeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;
        private readonly IWebHostEnvironment _env;

        public BelgeController(AppDbContext db, UserManager<Uye> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // ── GET /Belge/Index ──────────────────────────────────────────
        public async Task<IActionResult> Index(string? kategori)
        {
            var q = _db.Belgeler
                .Include(b => b.Yukleyen)
                .AsQueryable();

            // Erişim kontrolü
            if (!User.IsInRole("Admin") && !User.IsInRole("Yonetim"))
                q = q.Where(b => b.ErisimSeviyesi != BelgeErisim.SadeceYonetim);

            if (!string.IsNullOrWhiteSpace(kategori))
                q = q.Where(b => b.Kategori == kategori);

            var liste = await q.OrderByDescending(b => b.YuklemeTarihi).ToListAsync();

            var kategoriler = await _db.Belgeler
                .Select(b => b.Kategori)
                .Distinct()
                .ToListAsync();

            ViewBag.Kategori = kategori;
            ViewBag.Kategoriler = kategoriler;

            return View(liste);
        }

        // ── GET /Belge/Yukle ──────────────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public IActionResult Yukle()
        {
            return View(new BelgeYukleViewModel());
        }

        // ── POST /Belge/Yukle ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Yukle(BelgeYukleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Dosya == null || model.Dosya.Length == 0)
            {
                ModelState.AddModelError("Dosya", "Lütfen bir dosya seçin.");
                return View(model);
            }

            // Dosyayı kaydet
            var klasor = Path.Combine(_env.WebRootPath, "uploads", "belgeler");
            Directory.CreateDirectory(klasor);

            var uzanti = Path.GetExtension(model.Dosya.FileName).ToLower();
            var dosyaAdi = $"{Guid.NewGuid()}{uzanti}";
            var yol = Path.Combine(klasor, dosyaAdi);

            using var stream = new FileStream(yol, FileMode.Create);
            await model.Dosya.CopyToAsync(stream);

            var belge = new Belge
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Kategori = model.Kategori,
                DosyaYolu = $"/uploads/belgeler/{dosyaAdi}",
                DosyaAdi = model.Dosya.FileName,
                DosyaTipi = uzanti.TrimStart('.'),
                DosyaBoyutu = model.Dosya.Length,
                ErisimSeviyesi = model.ErisimSeviyesi,
                YukleyenId = _userManager.GetUserId(User)!,
                YuklemeTarihi = DateTime.UtcNow
            };

            _db.Belgeler.Add(belge);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Belge başarıyla yüklendi.";
            return RedirectToAction("Index");
        }

        // ── POST /Belge/Sil ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Sil(int id)
        {
            var belge = await _db.Belgeler.FindAsync(id);
            if (belge != null)
            {
                // Fiziksel dosyayı sil
                var fizikselYol = Path.Combine(_env.WebRootPath, belge.DosyaYolu.TrimStart('/'));
                if (System.IO.File.Exists(fizikselYol))
                    System.IO.File.Delete(fizikselYol);

                _db.Belgeler.Remove(belge);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "Belge silindi.";
            return RedirectToAction("Index");
        }
    }
}