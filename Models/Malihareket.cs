namespace DernekYonetim.Models
{
    public class MaliHareket
    {
        public int Id { get; set; }
        public MaliHareketTuru Tur { get; set; }
        public string Kategori { get; set; } = string.Empty;    // Aidat, Bağış, Kira vb.
        public string Aciklama { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string? BelgeUrl { get; set; }                    // Fatura/makbuz

        public string KaydEdenId { get; set; } = string.Empty;
        public virtual Uye? KaydEden { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }

    public enum MaliHareketTuru
    {
        Gelir,
        Gider
    }
}