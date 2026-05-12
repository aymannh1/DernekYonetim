namespace DernekYonetim.Models
{
    public class Bildirim
    {
        public int Id { get; set; }
        public string UyeId { get; set; } = string.Empty;
        public virtual Uye? Uye { get; set; }

        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public BildirimTuru Tur { get; set; } = BildirimTuru.Genel;
        public bool OkunduMu { get; set; } = false;
        public string? YonlendirmeUrl { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }

    public class LogKaydi
    {
        public int Id { get; set; }
        public string? UyeId { get; set; }
        public virtual Uye? Uye { get; set; }

        public string Olay { get; set; } = string.Empty;        // "uye_guncellendi" vb.
        public string? Detay { get; set; }
        public string? IpAdresi { get; set; }
        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }

    public enum BildirimTuru
    {
        Genel,
        Aidat,
        Etkinlik,
        Duyuru,
        Basvuru,
        Sistem
    }
}