using Microsoft.AspNetCore.Identity;

namespace DernekYonetim.Models
{
    /// <summary>
    /// IdentityUser'ı extend eder → AspNetUsers tablosuna eklenir.
    /// </summary>
    public class Uye : IdentityUser
    {
        public string UyeNo { get; set; } = string.Empty;       // UYE-2026-0001
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string? TcKimlik { get; set; }                    // AES-256 şifreli
        public string? Adres { get; set; }
        public string? Meslek { get; set; }
        public string? ProfilFotoUrl { get; set; }

        public UyelikDurumu UyelikDurumu { get; set; } = UyelikDurumu.Aktif;
        public DateTime UyelikTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? SonGiris { get; set; }

        // İlişkiler
        public virtual ICollection<Aidat> Aidatlar { get; set; } = [];
        public virtual ICollection<EtkinlikKayit> EtkinlikKayitlari { get; set; } = [];
        public virtual ICollection<Bildirim> Bildirimler { get; set; } = [];
        public virtual ICollection<Belge> Belgeler { get; set; } = [];

        // Hesaplanan özellik
        public string TamAd => $"{Ad} {Soyad}";
    }

    public enum UyelikDurumu
    {
        Aktif,
        Pasif,
        Askida,
        Fahri
    }
}