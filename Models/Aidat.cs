namespace DernekYonetim.Models
{
    public class Aidat
    {
        public int Id { get; set; }

        // FK → Uye
        public string UyeId { get; set; } = string.Empty;
        public virtual Uye? Uye { get; set; }

        public string Donem { get; set; } = string.Empty;       // "2026-01"
        public decimal Tutar { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime? OdemeTarihi { get; set; }

        public OdemeDurumu OdemeDurumu { get; set; } = OdemeDurumu.Bekliyor;
        public OdemeYontemi? OdemeYontemi { get; set; }
        public string? DekontUrl { get; set; }
        public bool DekontOnaylandi { get; set; } = false;
        public string? OnaylayanId { get; set; }
        public string? Aciklama { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Gecikme kontrolü
        public bool GecikiyorMu => OdemeDurumu == OdemeDurumu.Bekliyor
                                   && DateTime.UtcNow > SonOdemeTarihi;
    }

    public enum OdemeDurumu
    {
        Bekliyor,
        Odendi,
        Gecikti,
        Iptal
    }

    public enum OdemeYontemi
    {
        Nakit,
        KrediKarti,
        HavaleEft,
        Online
    }
}