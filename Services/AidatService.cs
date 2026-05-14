using DernekYonetim.Data;
using DernekYonetim.Models;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Services
{
    public class AidatService
    {
        private readonly AppDbContext _db;

        public AidatService(AppDbContext db)
        {
            _db = db;
        }

        // ── Tüm aktif üyeler için aidat oluştur ───────────────────────
        public async Task<int> TopluAidatOlusturAsync(
            string donem, decimal tutar, DateTime sonOdemeTarihi)
        {
            var sonOdemeTarihiUtc = DateTime.SpecifyKind(sonOdemeTarihi, DateTimeKind.Utc);

            var aktifUyeler = await _db.Users
                .Where(u => u.UyelikDurumu == UyelikDurumu.Aktif)
                .ToListAsync();

            int sayac = 0;
            foreach (var uye in aktifUyeler)
            {
                var mevcutMu = await _db.Aidatlar
                    .AnyAsync(a => a.UyeId == uye.Id && a.Donem == donem);

                if (!mevcutMu)
                {
                    _db.Aidatlar.Add(new Aidat
                    {
                        UyeId = uye.Id,
                        Donem = donem,
                        Tutar = tutar,
                        SonOdemeTarihi = sonOdemeTarihiUtc,
                        OdemeDurumu = OdemeDurumu.Bekliyor,
                        OlusturmaTarihi = DateTime.UtcNow
                    });
                    sayac++;
                }
            }

            await _db.SaveChangesAsync();
            return sayac;
        }

        // ── Aidat listesi (admin) ──────────────────────────────────────
        public async Task<List<Aidat>> ListeleAsync(
            string? donem = null, OdemeDurumu? durum = null)
        {
            var q = _db.Aidatlar
                .Include(a => a.Uye)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(donem))
                q = q.Where(a => a.Donem == donem);

            if (durum.HasValue)
                q = q.Where(a => a.OdemeDurumu == durum.Value);

            return await q.OrderBy(a => a.Donem)
                          .ThenBy(a => a.Uye!.Soyad)
                          .ToListAsync();
        }

        // ── Üyenin kendi aidatları ─────────────────────────────────────
        public async Task<List<Aidat>> UyeAidatlariAsync(string uyeId)
        {
            return await _db.Aidatlar
                .Where(a => a.UyeId == uyeId)
                .OrderByDescending(a => a.Donem)
                .ToListAsync();
        }

        // ── Dekont yükle ───────────────────────────────────────────────
        public async Task<bool> DekontYukleAsync(
            int aidatId, string uyeId, string dekontUrl, OdemeYontemi yontem)
        {
            var aidat = await _db.Aidatlar
                .FirstOrDefaultAsync(a => a.Id == aidatId && a.UyeId == uyeId);

            if (aidat == null) return false;

            aidat.DekontUrl = dekontUrl;
            aidat.OdemeYontemi = yontem;
            aidat.OdemeDurumu = OdemeDurumu.Bekliyor;
            aidat.OdemeTarihi = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        // ── Dekontu onayla ─────────────────────────────────────────────
        public async Task<bool> DekontOnaylaAsync(int aidatId, string adminId)
        {
            var aidat = await _db.Aidatlar.FindAsync(aidatId);
            if (aidat == null) return false;

            aidat.DekontOnaylandi = true;
            aidat.OdemeDurumu = OdemeDurumu.Odendi;
            aidat.OnaylayanId = adminId;

            await _db.SaveChangesAsync();
            return true;
        }

        // ── Gecikenleri güncelle ───────────────────────────────────────
        public async Task<int> GecikmislerGuncelleAsync()
        {
            var gecikenler = await _db.Aidatlar
                .Where(a => a.OdemeDurumu == OdemeDurumu.Bekliyor
                         && a.SonOdemeTarihi < DateTime.UtcNow)
                .ToListAsync();

            foreach (var a in gecikenler)
                a.OdemeDurumu = OdemeDurumu.Gecikti;

            await _db.SaveChangesAsync();
            return gecikenler.Count;
        }

        // ── Dönem listesi (dropdown için) ──────────────────────────────
        public async Task<List<string>> DonemListesiAsync()
        {
            return await _db.Aidatlar
                .Select(a => a.Donem)
                .Distinct()
                .OrderByDescending(d => d)
                .ToListAsync();
        }

        // ── İstatistik ─────────────────────────────────────────────────
        public async Task<AidatIstatistik> IstatistikAsync(string? donem = null)
        {
            var q = _db.Aidatlar.AsQueryable();
            if (!string.IsNullOrWhiteSpace(donem))
                q = q.Where(a => a.Donem == donem);

            return new AidatIstatistik
            {
                Toplam = await q.CountAsync(),
                Odendi = await q.CountAsync(a => a.OdemeDurumu == OdemeDurumu.Odendi),
                Bekliyor = await q.CountAsync(a => a.OdemeDurumu == OdemeDurumu.Bekliyor),
                Gecikti = await q.CountAsync(a => a.OdemeDurumu == OdemeDurumu.Gecikti),
                ToplamTutar = await q.Where(a => a.OdemeDurumu == OdemeDurumu.Odendi).SumAsync(a => a.Tutar),
                BekleyenTutar = await q.Where(a => a.OdemeDurumu != OdemeDurumu.Odendi).SumAsync(a => a.Tutar)
            };
        }
    }

    public class AidatIstatistik
    {
        public int Toplam { get; set; }
        public int Odendi { get; set; }
        public int Bekliyor { get; set; }
        public int Gecikti { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal BekleyenTutar { get; set; }
    }
}