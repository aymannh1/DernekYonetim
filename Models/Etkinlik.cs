namespace DernekYonetim.Models
{
    public class Etkinlik
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public EtkinlikKategori Kategori { get; set; }
        public string? Yer { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int? Kontenjan { get; set; }                      // null = sınırsız
        public string? GorselUrl { get; set; }
        public bool UcretsizMi { get; set; } = true;
        public decimal? Ucret { get; set; }
        public EtkinlikDurumu Durum { get; set; } = EtkinlikDurumu.Taslak;

        public string OlusturanId { get; set; } = string.Empty;
        public virtual Uye? Olusturan { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        public virtual ICollection<EtkinlikKayit> Kayitlar { get; set; } = [];

        // Kalan kontenjan
        public int? KalanKontenjan => Kontenjan.HasValue
            ? Kontenjan.Value - Kayitlar.Count(k => k.Durum == EtkinlikKayitDurumu.Onaylandi)
            : null;
    }
    public class EtkinlikKayit
    {
        public int Id { get; set; }
        public string UyeId { get; set; } = string.Empty;
        public virtual Uye? Uye { get; set; }
        public int EtkinlikId { get; set; }
        public virtual Etkinlik? Etkinlik { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
        public EtkinlikKayitDurumu Durum { get; set; } = EtkinlikKayitDurumu.Onaylandi;
        public bool KatildiMi { get; set; } = false;
    }

    public enum EtkinlikKategori
    {
        Seminer,
        Toplanti,
        Sosyal,
        Spor,
        Egitim
    }

    public enum EtkinlikDurumu
    {
        Taslak,
        Yayinda,
        Iptal,
        Tamamlandi
    }

    public enum EtkinlikKayitDurumu
    {
        Onaylandi,
        IptalEdildi,
        Bekleniyor
    }
}
