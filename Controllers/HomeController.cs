using DernekYonetim.Data;
using DernekYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Yonetim"))
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Uye");
            }

            // Kamuoyuna açık duyurular
            var duyurular = await _db.Duyurular
                .Where(d => d.Aktif && d.KamuoyunaAcik &&
                       (d.YayinTarihi == null || d.YayinTarihi <= DateTime.UtcNow) &&
                       (d.BitisTarihi == null || d.BitisTarihi >= DateTime.UtcNow))
                .OrderByDescending(d => d.OlusturmaTarihi)
                .Take(3)
                .ToListAsync();

            // Yaklaşan etkinlikler
            var etkinlikler = await _db.Etkinlikler
                .Where(e => e.Durum == EtkinlikDurumu.Yayinda &&
                            e.BaslangicTarihi >= DateTime.UtcNow)
                .OrderBy(e => e.BaslangicTarihi)
                .Take(3)
                .ToListAsync();

            ViewBag.Duyurular = duyurular;
            ViewBag.Etkinlikler = etkinlikler;

            return View();
        }
    }
}