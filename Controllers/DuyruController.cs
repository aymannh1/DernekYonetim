using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    public class DuyuruController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;

        public DuyuruController(AppDbContext db, UserManager<Uye> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET /Duyuru/Index ─────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var duyurular = await _db.Duyurular
                .Where(d => d.Aktif &&
                       (d.YayinTarihi == null || d.YayinTarihi <= DateTime.UtcNow) &&
                       (d.BitisTarihi == null || d.BitisTarihi >= DateTime.UtcNow))
                .OrderByDescending(d => d.OlusturmaTarihi)
                .ToListAsync();

            return View(duyurular);
        }

        // ── GET /Duyuru/Detay/5 ───────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Detay(int id)
        {
            var duyuru = await _db.Duyurular
                .FirstOrDefaultAsync(d => d.Id == id);

            if (duyuru == null) return NotFound();
            return View(duyuru);
        }

        // ── GET /Duyuru/Olustur ───────────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public IActionResult Olustur()
        {
            return View(new DuyuruViewModel());
        }

        // ── POST /Duyuru/Olustur ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Olustur(DuyuruViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var duyuru = new Duyuru
            {
                Baslik = model.Baslik,
                Icerik = model.Icerik,
                Ozet = model.Ozet,
                Hedef = model.Hedef,
                KamuoyunaAcik = model.KamuoyunaAcik,
                YayinTarihi = model.YayinTarihi.HasValue ? DateTime.SpecifyKind(model.YayinTarihi.Value, DateTimeKind.Utc) : (DateTime?)null,
                BitisTarihi = model.BitisTarihi.HasValue ? DateTime.SpecifyKind(model.BitisTarihi.Value, DateTimeKind.Utc) : (DateTime?)null,
                Aktif = true,
                OlusturanId = _userManager.GetUserId(User)!,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _db.Duyurular.Add(duyuru);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Duyuru oluşturuldu.";
            return RedirectToAction("Yonetim");
        }

        // ── GET /Duyuru/Duzenle/5 ─────────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var duyuru = await _db.Duyurular.FindAsync(id);
            if (duyuru == null) return NotFound();

            var model = new DuyuruViewModel
            {
                Id = duyuru.Id,
                Baslik = duyuru.Baslik,
                Icerik = duyuru.Icerik,
                Ozet = duyuru.Ozet,
                Hedef = duyuru.Hedef,
                KamuoyunaAcik = duyuru.KamuoyunaAcik,
                YayinTarihi = duyuru.YayinTarihi,
                BitisTarihi = duyuru.BitisTarihi,
                Aktif = duyuru.Aktif
            };
            return View(model);
        }

        // ── POST /Duyuru/Duzenle ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(DuyuruViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var duyuru = await _db.Duyurular.FindAsync(model.Id);
            if (duyuru == null) return NotFound();

            duyuru.Baslik = model.Baslik;
            duyuru.Icerik = model.Icerik;
            duyuru.Ozet = model.Ozet;
            duyuru.Hedef = model.Hedef;
            duyuru.KamuoyunaAcik = model.KamuoyunaAcik;
            duyuru.YayinTarihi = model.YayinTarihi.HasValue ? DateTime.SpecifyKind(model.YayinTarihi.Value, DateTimeKind.Utc) : (DateTime?)null;
            duyuru.BitisTarihi = model.BitisTarihi.HasValue ? DateTime.SpecifyKind(model.BitisTarihi.Value, DateTimeKind.Utc) : (DateTime?)null;
            duyuru.Aktif = model.Aktif;

            await _db.SaveChangesAsync();
            TempData["Basari"] = "Duyuru güncellendi.";
            return RedirectToAction("Yonetim");
        }

        // ── POST /Duyuru/Sil ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Sil(int id)
        {
            var duyuru = await _db.Duyurular.FindAsync(id);
            if (duyuru != null)
            {
                _db.Duyurular.Remove(duyuru);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "Duyuru silindi.";
            return RedirectToAction("Yonetim");
        }

        // ── GET /Duyuru/Yonetim (Admin) ───────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Yonetim()
        {
            var duyurular = await _db.Duyurular
                .OrderByDescending(d => d.OlusturmaTarihi)
                .ToListAsync();
            return View(duyurular);
        }
    }
}