namespace DernekYonetim.Models
{
    public class Duyuru
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;       // HTML rich text
        public string? Ozet { get; set; }
        public string? GorselUrl { get; set; }

        public DuyuruHedef Hedef { get; set; } = DuyuruHedef.TumUyeler;
        public bool KamuoyunaAcik { get; set; } = false;         // Ana sayfada göster

        public DateTime? YayinTarihi { get; set; }               // Zamanlanmış yayın
        public DateTime? BitisTarihi { get; set; }
        public bool Aktif { get; set; } = true;

        public string OlusturanId { get; set; } = string.Empty;
        public virtual Uye? Olusturan { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }

    public enum DuyuruHedef
    {
        TumUyeler,
        SadeceYonetim,
        BelirliGrup
    }
}