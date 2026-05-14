using DernekYonetim.Data;
using DernekYonetim.Models;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Services
{
    public class EtkinlikService
    {
        private readonly AppDbContext _db;

        public EtkinlikService(AppDbContext db)
        {
            _db = db;
        }

        // ── Etkinlik listesi ───────────────────────────────────────────
        public async Task<List<Etkinlik>> ListeleAsync(bool sadeceyayinda = false)
        {
            var q = _db.Etkinlikler
                .Include(e => e.Kayitlar)
                .AsQueryable();

            if (sadeceyayinda)
                q = q.Where(e => e.Durum == EtkinlikDurumu.Yayinda
                              && e.BitisTarihi >= DateTime.UtcNow);

            return await q.OrderBy(e => e.BaslangicTarihi).ToListAsync();
        }

        // ── Tek etkinlik ───────────────────────────────────────────────
        public async Task<Etkinlik?> BulAsync(int id)
        {
            return await _db.Etkinlikler
                .Include(e => e.Kayitlar)
                    .ThenInclude(k => k.Uye)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ── Etkinlik oluştur ───────────────────────────────────────────
        public async Task<Etkinlik> OlusturAsync(Etkinlik etkinlik)
        {
            _db.Etkinlikler.Add(etkinlik);
            await _db.SaveChangesAsync();
            return etkinlik;
        }

        // ── Etkinlik güncelle ──────────────────────────────────────────
        public async Task<bool> GuncelleAsync(Etkinlik etkinlik)
        {
            _db.Etkinlikler.Update(etkinlik);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Etkinlik sil ───────────────────────────────────────────────
        public async Task<bool> SilAsync(int id)
        {
            var etkinlik = await _db.Etkinlikler.FindAsync(id);
            if (etkinlik == null) return false;
            _db.Etkinlikler.Remove(etkinlik);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Kayıt ol ───────────────────────────────────────────────────
        public async Task<(bool Basarili, string Mesaj)> KayitOlAsync(int etkinlikId, string uyeId)
        {
            var etkinlik = await _db.Etkinlikler
                .Include(e => e.Kayitlar)
                .FirstOrDefaultAsync(e => e.Id == etkinlikId);

            if (etkinlik == null)
                return (false, "Etkinlik bulunamadı.");

            if (etkinlik.Durum != EtkinlikDurumu.Yayinda)
                return (false, "Bu etkinliğe kayıt yapılamaz.");

            var mevcutKayit = etkinlik.Kayitlar
                .Any(k => k.UyeId == uyeId && k.Durum == EtkinlikKayitDurumu.Onaylandi);

            if (mevcutKayit)
                return (false, "Zaten bu etkinliğe kayıtlısınız.");

            if (etkinlik.Kontenjan.HasValue)
            {
                var doluKontenjan = etkinlik.Kayitlar
                    .Count(k => k.Durum == EtkinlikKayitDurumu.Onaylandi);
                if (doluKontenjan >= etkinlik.Kontenjan.Value)
                    return (false, "Etkinlik kontenjanı doldu.");
            }

            var kayit = new EtkinlikKayit
            {
                EtkinlikId = etkinlikId,
                UyeId = uyeId,
                KayitTarihi = DateTime.UtcNow,
                Durum = EtkinlikKayitDurumu.Onaylandi
            };

            _db.EtkinlikKayitlari.Add(kayit);
            await _db.SaveChangesAsync();
            return (true, "Etkinliğe başarıyla kayıt oldunuz.");
        }

        // ── Kayıt iptal ────────────────────────────────────────────────
        public async Task<bool> KayitIptalAsync(int etkinlikId, string uyeId)
        {
            var kayit = await _db.EtkinlikKayitlari
                .FirstOrDefaultAsync(k => k.EtkinlikId == etkinlikId && k.UyeId == uyeId);

            if (kayit == null) return false;

            kayit.Durum = EtkinlikKayitDurumu.IptalEdildi;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}