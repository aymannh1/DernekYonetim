using DernekYonetim.Data;
using DernekYonetim.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DernekYonetim.Services
{
    public class UyeService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Uye> _userManager;
        private readonly IConfiguration _cfg;

        public UyeService(AppDbContext db, UserManager<Uye> userManager, IConfiguration cfg)
        {
            _db = db;
            _userManager = userManager;
            _cfg = cfg;
        }

        // ── Üye No üret ────────────────────────────────────────────────
        public async Task<string> UyeNoUretAsync()
        {
            var prefix = _cfg["AppSettings:UyeNoPrefix"] ?? "UYE";
            var yil = DateTime.Now.Year;
            var sayi = await _db.Users
                             .CountAsync(u => u.UyeNo.StartsWith($"{prefix}-{yil}-"));
            return $"{prefix}-{yil}-{(sayi + 1):D4}";
        }

        // ── Üye listesi (filtreli + sayfalı) ──────────────────────────
        public async Task<(List<Uye> Liste, int ToplamSayfa)> ListeleAsync(
            string? arama = null,
            UyelikDurumu? durum = null,
            int sayfa = 1,
            int boyut = 20)
        {
            var q = _db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(arama))
            {
                var s = arama.ToLower();
                q = q.Where(u => u.Ad.ToLower().Contains(s)
                               || u.Soyad.ToLower().Contains(s)
                               || u.Email!.ToLower().Contains(s)
                               || u.UyeNo.ToLower().Contains(s));
            }

            if (durum.HasValue)
                q = q.Where(u => u.UyelikDurumu == durum.Value);

            var toplam = await q.CountAsync();
            var liste = await q
                .OrderBy(u => u.Soyad).ThenBy(u => u.Ad)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .ToListAsync();

            return (liste, (int)Math.Ceiling(toplam / (double)boyut));
        }

        // ── Tek üye (ilişkilerle) ──────────────────────────────────────
        public async Task<Uye?> BulAsync(string id) =>
            await _db.Users
                .Include(u => u.Aidatlar)
                .Include(u => u.EtkinlikKayitlari).ThenInclude(ek => ek.Etkinlik)
                .Include(u => u.Bildirimler.Where(b => !b.OkunduMu))
                .FirstOrDefaultAsync(u => u.Id == id);

        // ── Başvuruyu onayla ve hesap oluştur ─────────────────────────
        public async Task<IdentityResult> BasvuruOnaylaAsync(int basvuruId, string adminId)
        {
            var b = await _db.Basvurular.FindAsync(basvuruId);
            if (b == null)
                return IdentityResult.Failed(new IdentityError { Description = "Başvuru bulunamadı." });

            var uye = new Uye
            {
                UserName = b.Email,
                Email = b.Email,
                EmailConfirmed = true,
                Ad = b.Ad,
                Soyad = b.Soyad,
                Telefon = b.Telefon,
                TcKimlik = b.TcKimlik,
                Adres = b.Adres,
                Meslek = b.Meslek,
                UyeNo = await UyeNoUretAsync(),
                UyelikDurumu = UyelikDurumu.Aktif
            };

            // Geçici şifre – üye ilk girişte değiştirecek
            var geciciSifre = $"Dernek@{DateTime.Now.Year}!";
            var sonuc = await _userManager.CreateAsync(uye, geciciSifre);

            if (sonuc.Succeeded)
            {
                await _userManager.AddToRoleAsync(uye, DbSeeder.RolUye);
                b.Durum = BasvuruDurumu.Onaylandi;
                b.IslemTarihi = DateTime.UtcNow;
                b.IslemYapanId = adminId;
                await _db.SaveChangesAsync();
            }

            return sonuc;
        }

        // ── Üyelik durumu güncelle ─────────────────────────────────────
        public async Task<bool> DurumGuncelleAsync(string uyeId, UyelikDurumu yeni)
        {
            var uye = await _db.Users.FindAsync(uyeId);
            if (uye == null) return false;
            uye.UyelikDurumu = yeni;
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Dashboard istatistikleri ───────────────────────────────────
        public async Task<UyeIstatistik> IstatistikGetirAsync()
        {
            var ayBasi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return new UyeIstatistik
            {
                Toplam = await _db.Users.CountAsync(),
                Aktif = await _db.Users.CountAsync(u => u.UyelikDurumu == UyelikDurumu.Aktif),
                BuAyYeni = await _db.Users.CountAsync(u => u.UyelikTarihi >= ayBasi),
                BekleyenBasvuru = await _db.Basvurular.CountAsync(b => b.Durum == BasvuruDurumu.Bekliyor)
            };
        }
    }

    public class UyeIstatistik
    {
        public int Toplam { get; set; }
        public int Aktif { get; set; }
        public int BuAyYeni { get; set; }
        public int BekleyenBasvuru { get; set; }
    }
}