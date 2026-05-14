using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    [Authorize(Roles = "Admin,Yonetim")]
    public class MaliController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;

        public MaliController(AppDbContext db, UserManager<Uye> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET /Mali/Index ───────────────────────────────────────────
        public async Task<IActionResult> Index(int? yil, MaliHareketTuru? tur)
        {
            var seciliYil = yil ?? DateTime.UtcNow.Year;

            var q = _db.MaliHareketler
                .Include(m => m.KaydEden)
                .Where(m => m.IslemTarihi.Year == seciliYil)
                .AsQueryable();

            if (tur.HasValue)
                q = q.Where(m => m.Tur == tur.Value);

            var liste = await q
                .OrderByDescending(m => m.IslemTarihi)
                .ToListAsync();

            // İstatistikler
            var tumHareketler = await _db.MaliHareketler
                .Where(m => m.IslemTarihi.Year == seciliYil)
                .ToListAsync();

            ViewBag.ToplamGelir = tumHareketler
                .Where(m => m.Tur == MaliHareketTuru.Gelir).Sum(m => m.Tutar);
            ViewBag.ToplamGider = tumHareketler
                .Where(m => m.Tur == MaliHareketTuru.Gider).Sum(m => m.Tutar);
            ViewBag.NetBakiye = (decimal)ViewBag.ToplamGelir - (decimal)ViewBag.ToplamGider;
            ViewBag.SeciliYil = seciliYil;
            ViewBag.SeciliTur = tur;

            return View(liste);
        }

        // ── GET /Mali/Ekle ────────────────────────────────────────────
        public IActionResult Ekle()
        {
            return View(new MaliHareketViewModel());
        }

        // ── POST /Mali/Ekle ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(MaliHareketViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hareket = new MaliHareket
            {
                Tur = model.Tur,
                Kategori = model.Kategori,
                Aciklama = model.Aciklama,
                Tutar = model.Tutar,
                IslemTarihi = model.IslemTarihi,
                KaydEdenId = _userManager.GetUserId(User)!,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _db.MaliHareketler.Add(hareket);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Mali hareket kaydedildi.";
            return RedirectToAction("Index");
        }

        // ── POST /Mali/Sil ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var hareket = await _db.MaliHareketler.FindAsync(id);
            if (hareket != null)
            {
                _db.MaliHareketler.Remove(hareket);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "Kayıt silindi.";
            return RedirectToAction("Index");
        }

        // ── GET /Mali/Rapor ───────────────────────────────────────────
        public async Task<IActionResult> Rapor()
        {
            var yil = DateTime.UtcNow.Year;

            var aylikRapor = await _db.MaliHareketler
                .Where(m => m.IslemTarihi.Year == yil)
                .GroupBy(m => new { m.IslemTarihi.Month, m.Tur })
                .Select(g => new
                {
                    Ay = g.Key.Month,
                    Tur = g.Key.Tur,
                    Top = g.Sum(m => m.Tutar)
                })
                .ToListAsync();

            var aylar = Enumerable.Range(1, 12).Select(ay => new
            {
                Ay = ay,
                AyAdi = new DateTime(yil, ay, 1).ToString("MMMM", new System.Globalization.CultureInfo("tr-TR")),
                Gelir = aylikRapor
                    .Where(r => r.Ay == ay && r.Tur == MaliHareketTuru.Gelir)
                    .Sum(r => r.Top),
                Gider = aylikRapor
                    .Where(r => r.Ay == ay && r.Tur == MaliHareketTuru.Gider)
                    .Sum(r => r.Top)
            }).ToList();

            ViewBag.AylikRapor = aylar;
            ViewBag.Yil = yil;
            ViewBag.ToplamGelir = aylar.Sum(a => a.Gelir);
            ViewBag.ToplamGider = aylar.Sum(a => a.Gider);

            return View();
        }
    }
}