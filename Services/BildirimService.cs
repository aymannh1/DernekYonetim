using DernekYonetim.Data;
using DernekYonetim.Models;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Services
{
    public class BildirimService
    {
        private readonly AppDbContext _db;

        public BildirimService(AppDbContext db)
        {
            _db = db;
        }

        public async Task EkleAsync(
            string uyeId, string baslik, string mesaj,
            BildirimTuru tur = BildirimTuru.Genel, string? url = null)
        {
            _db.Bildirimler.Add(new Bildirim
            {
                UyeId = uyeId,
                Baslik = baslik,
                Mesaj = mesaj,
                Tur = tur,
                YonlendirmeUrl = url,
                OlusturmaTarihi = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        public async Task<(int Basarili, int Basarisiz)> TopluEkleAsync(
            List<string> uyeIdler, string baslik, string mesaj,
            BildirimTuru tur = BildirimTuru.Genel, string? url = null)
        {
            if (uyeIdler.Count == 0) return (0, 0);

            var bildirimler = uyeIdler.Select(id => new Bildirim
            {
                UyeId = id,
                Baslik = baslik,
                Mesaj = mesaj,
                Tur = tur,
                YonlendirmeUrl = url,
                OlusturmaTarihi = DateTime.UtcNow
            }).ToList();

            _db.Bildirimler.AddRange(bildirimler);
            await _db.SaveChangesAsync();
            return (bildirimler.Count, 0);
        }

        public async Task<int> OkunmamisSayiAsync(string uyeId) =>
            await _db.Bildirimler.CountAsync(b => b.UyeId == uyeId && !b.OkunduMu);

        public async Task<List<Bildirim>> ListeleAsync(string uyeId) =>
            await _db.Bildirimler
                .Where(b => b.UyeId == uyeId)
                .OrderByDescending(b => b.OlusturmaTarihi)
                .ToListAsync();

        public async Task OkunduIsaretleAsync(int id, string uyeId)
        {
            var bildirim = await _db.Bildirimler
                .FirstOrDefaultAsync(b => b.Id == id && b.UyeId == uyeId);
            if (bildirim != null && !bildirim.OkunduMu)
            {
                bildirim.OkunduMu = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task TumunuOkunduIsaretleAsync(string uyeId)
        {
            await _db.Bildirimler
                .Where(b => b.UyeId == uyeId && !b.OkunduMu)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.OkunduMu, true));
        }
    }
}
