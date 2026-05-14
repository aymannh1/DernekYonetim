namespace DernekYonetim.Models
{
    public class Belge
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string Kategori { get; set; } = string.Empty;    // Tüzük, Yönetmelik, Tutanak, Form
        public string DosyaYolu { get; set; } = string.Empty;
        public byte[]? DosyaIcerigi { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public string DosyaTipi { get; set; } = string.Empty;   // pdf, docx, xlsx
        public long DosyaBoyutu { get; set; }
        public int Versiyon { get; set; } = 1;
        public int? OncekiVersiyonId { get; set; }

        public BelgeErisim ErisimSeviyesi { get; set; } = BelgeErisim.UyeyeOzel;

        public string YukleyenId { get; set; } = string.Empty;
        public virtual Uye? Yukleyen { get; set; }
        public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
    }

    public enum BelgeErisim
    {
        HerkesAcik,
        UyeyeOzel,
        SadeceYonetim
    }
}