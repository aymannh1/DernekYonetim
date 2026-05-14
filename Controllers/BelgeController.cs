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

        public BelgeController(AppDbContext db, UserManager<Uye> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET /Belge/Index ──────────────────────────────────────────
        public async Task<IActionResult> Index(string? kategori)
        {
            var q = _db.Belgeler
                .Include(b => b.Yukleyen)
                .AsQueryable();

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

        // ── GET /Belge/Indir/5 ────────────────────────────────────────
        public async Task<IActionResult> Indir(int id)
        {
            var belge = await _db.Belgeler.FindAsync(id);
            if (belge == null || belge.DosyaIcerigi == null)
                return NotFound();

            if (belge.ErisimSeviyesi == BelgeErisim.SadeceYonetim
                && !User.IsInRole("Admin") && !User.IsInRole("Yonetim"))
                return Forbid();

            var contentType = belge.DosyaTipi switch
            {
                "pdf"  => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png"  => "image/png",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "doc"  => "application/msword",
                "xls"  => "application/vnd.ms-excel",
                _      => "application/octet-stream"
            };

            return File(belge.DosyaIcerigi, contentType, belge.DosyaAdi);
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

            using var ms = new MemoryStream();
            await model.Dosya.CopyToAsync(ms);
            var icerik = ms.ToArray();

            var uzanti = Path.GetExtension(model.Dosya.FileName).ToLower().TrimStart('.');

            var belge = new Belge
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Kategori = model.Kategori,
                DosyaYolu = string.Empty,
                DosyaIcerigi = icerik,
                DosyaAdi = model.Dosya.FileName,
                DosyaTipi = uzanti,
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
                _db.Belgeler.Remove(belge);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "Belge silindi.";
            return RedirectToAction("Index");
        }
    }
}
