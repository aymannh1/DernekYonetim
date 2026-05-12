using DernekYonetim.Data;
using DernekYonetim.Models;
using DernekYonetim.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Controllers
{
    public class ProjeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProjeController(AppDbContext db, UserManager<Uye> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // ── GET /Proje/Index ──────────────────────────────────────────
        public async Task<IActionResult> Index(ProjeKategori? kategori, ProjeDurumu? durum, string? ulke)
        {
            var q = _db.Projeler
                .Include(p => p.Belgeler)
                .Include(p => p.Guncellemeler)
                .AsQueryable();

            if (kategori.HasValue) q = q.Where(p => p.Kategori == kategori.Value);
            if (durum.HasValue) q = q.Where(p => p.Durum == durum.Value);
            if (!string.IsNullOrWhiteSpace(ulke)) q = q.Where(p => p.Ulke == ulke);

            var projeler = await q.OrderByDescending(p => p.OlusturmaTarihi).ToListAsync();

            var ulkeler = await _db.Projeler.Select(p => p.Ulke).Distinct().ToListAsync();

            ViewBag.Kategori = kategori;
            ViewBag.Durum = durum;
            ViewBag.Ulke = ulke;
            ViewBag.Ulkeler = ulkeler;

            return View(projeler);
        }

        // ── GET /Proje/Detay/5 ────────────────────────────────────────
        public async Task<IActionResult> Detay(int id)
        {
            var proje = await _db.Projeler
                .Include(p => p.Belgeler).ThenInclude(b => b.Yukleyen)
                .Include(p => p.Guncellemeler).ThenInclude(g => g.Yazan)
                .Include(p => p.Olusturan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proje == null) return NotFound();
            return View(proje);
        }

        // ── GET /Proje/Olustur ────────────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public IActionResult Olustur()
        {
            return View(new ProjeViewModel());
        }

        // ── POST /Proje/Olustur ───────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Olustur(ProjeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? kapakUrl = null;
            if (model.KapakFoto != null && model.KapakFoto.Length > 0)
                kapakUrl = await DosyaKaydetAsync(model.KapakFoto, "kapaklar");

            var proje = new Proje
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Kategori = model.Kategori,
                DigerKategori = model.DigerKategori,
                Ulke = model.Ulke,
                Sehir = model.Sehir,
                HedefBütce = model.HedefButce,
                ToplamBagis = model.ToplamBagis,
                Durum = model.Durum,
                BaslangicTarihi = model.BaslangicTarihi,
                BitisTarihi = model.BitisTarihi,
                KapakFotoUrl = kapakUrl,
                OlusturanId = _userManager.GetUserId(User)!,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _db.Projeler.Add(proje);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Proje oluşturuldu.";
            return RedirectToAction("Detay", new { id = proje.Id });
        }

        // ── GET /Proje/Duzenle/5 ─────────────────────────────────────
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var proje = await _db.Projeler.FindAsync(id);
            if (proje == null) return NotFound();

            var model = new ProjeViewModel
            {
                Id = proje.Id,
                Baslik = proje.Baslik,
                Aciklama = proje.Aciklama,
                Kategori = proje.Kategori,
                DigerKategori = proje.DigerKategori,
                Ulke = proje.Ulke,
                Sehir = proje.Sehir,
                HedefButce = proje.HedefBütce,
                ToplamBagis = proje.ToplamBagis,
                Durum = proje.Durum,
                BaslangicTarihi = proje.BaslangicTarihi,
                BitisTarihi = proje.BitisTarihi
            };
            return View(model);
        }

        // ── POST /Proje/Duzenle ───────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Duzenle(ProjeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var proje = await _db.Projeler.FindAsync(model.Id);
            if (proje == null) return NotFound();

            if (model.KapakFoto != null && model.KapakFoto.Length > 0)
                proje.KapakFotoUrl = await DosyaKaydetAsync(model.KapakFoto, "kapaklar");

            proje.Baslik = model.Baslik;
            proje.Aciklama = model.Aciklama;
            proje.Kategori = model.Kategori;
            proje.DigerKategori = model.DigerKategori;
            proje.Ulke = model.Ulke;
            proje.Sehir = model.Sehir;
            proje.HedefBütce = model.HedefButce;
            proje.ToplamBagis = model.ToplamBagis;
            proje.Durum = model.Durum;
            proje.BaslangicTarihi = model.BaslangicTarihi;
            proje.BitisTarihi = model.BitisTarihi;

            await _db.SaveChangesAsync();
            TempData["Basari"] = "Proje güncellendi.";
            return RedirectToAction("Detay", new { id = proje.Id });
        }

        // ── POST /Proje/BelgeYukle ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> BelgeYukle(ProjeBelgeYukleViewModel model)
        {
            string dosyaYoluVeyaLink = string.Empty;

            if (model.Tur == ProjeBelgeTuru.Video)
            {
                if (string.IsNullOrWhiteSpace(model.VideoLink))
                {
                    TempData["Hata"] = "Video linki zorunludur.";
                    return RedirectToAction("Detay", new { id = model.ProjeId });
                }
                dosyaYoluVeyaLink = model.VideoLink;
            }
            else
            {
                if (model.Dosya == null || model.Dosya.Length == 0)
                {
                    TempData["Hata"] = "Dosya seçiniz.";
                    return RedirectToAction("Detay", new { id = model.ProjeId });
                }

                var klasor = model.Tur == ProjeBelgeTuru.Fotograf ? "fotograflar" : "proje-belgeler";
                dosyaYoluVeyaLink = await DosyaKaydetAsync(model.Dosya, klasor);
            }

            var belge = new ProjeBelge
            {
                ProjeId = model.ProjeId,
                Tur = model.Tur,
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                DosyaYoluVeyaLink = dosyaYoluVeyaLink,
                DosyaBoyutu = model.Dosya?.Length,
                YukleyenId = _userManager.GetUserId(User)!,
                YuklemeTarihi = DateTime.UtcNow
            };

            _db.ProjeBelgeleri.Add(belge);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "İçerik başarıyla eklendi.";
            return RedirectToAction("Detay", new { id = model.ProjeId });
        }

        // ── POST /Proje/BelgeSil ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> BelgeSil(int id, int projeId)
        {
            var belge = await _db.ProjeBelgeleri.FindAsync(id);
            if (belge != null)
            {
                if (belge.Tur != ProjeBelgeTuru.Video)
                {
                    var fizikselYol = Path.Combine(_env.WebRootPath,
                        belge.DosyaYoluVeyaLink.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(fizikselYol))
                        System.IO.File.Delete(fizikselYol);
                }
                _db.ProjeBelgeleri.Remove(belge);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "İçerik silindi.";
            return RedirectToAction("Detay", new { id = projeId });
        }

        // ── POST /Proje/GuncellemeEkle ────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> GuncellemeEkle(ProjeGuncellemeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Hata"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction("Detay", new { id = model.ProjeId });
            }

            var guncelleme = new ProjeGuncelleme
            {
                ProjeId = model.ProjeId,
                Baslik = model.Baslik,
                Icerik = model.Icerik,
                IlerlemeYuzdesi = model.IlerlemeYuzdesi,
                YazanId = _userManager.GetUserId(User)!,
                Tarih = DateTime.UtcNow
            };

            _db.ProjeGuncellemeleri.Add(guncelleme);
            await _db.SaveChangesAsync();

            TempData["Basari"] = "Güncelleme eklendi.";
            return RedirectToAction("Detay", new { id = model.ProjeId });
        }

        // ── POST /Proje/Sil ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Yonetim")]
        public async Task<IActionResult> Sil(int id)
        {
            var proje = await _db.Projeler.FindAsync(id);
            if (proje != null)
            {
                _db.Projeler.Remove(proje);
                await _db.SaveChangesAsync();
            }
            TempData["Basari"] = "Proje silindi.";
            return RedirectToAction("Index");
        }

        // ── YARDIMCI: Dosya kaydet ────────────────────────────────────
        private async Task<string> DosyaKaydetAsync(IFormFile dosya, string klasorAdi)
        {
            var klasor = Path.Combine(_env.WebRootPath, "uploads", "projeler", klasorAdi);
            Directory.CreateDirectory(klasor);
            var uzanti = Path.GetExtension(dosya.FileName).ToLower();
            var dosyaAdi = $"{Guid.NewGuid()}{uzanti}";
            var yol = Path.Combine(klasor, dosyaAdi);
            using var stream = new FileStream(yol, FileMode.Create);
            await dosya.CopyToAsync(stream);
            return $"/uploads/projeler/{klasorAdi}/{dosyaAdi}";
        }
    }
}