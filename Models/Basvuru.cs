namespace DernekYonetim.Models
{
    public class Basvuru
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string? TcKimlik { get; set; }
        public string? Adres { get; set; }
        public string? Meslek { get; set; }
        public string? BelgeUrl { get; set; }
        public string? BasvuruNotu { get; set; }

        public BasvuruDurumu Durum { get; set; } = BasvuruDurumu.Bekliyor;
        public DateTime BasvuruTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? IslemTarihi { get; set; }
        public string? IslemYapanId { get; set; }
        public string? RedNedeni { get; set; }
        public string? EkBilgiMesaji { get; set; }

        public string TamAd => $"{Ad} {Soyad}";
    }

    public enum BasvuruDurumu
    {
        Bekliyor,
        Onaylandi,
        Reddedildi,
        EkBilgiIstendi
    }
}
