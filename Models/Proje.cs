namespace DernekYonetim.Models
{
    public class Proje
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public ProjeKategori Kategori { get; set; }
        public string? DigerKategori { get; set; }      // Kategori = Diger ise
        public string Ulke { get; set; } = string.Empty;
        public string? Sehir { get; set; }
        public decimal? HedefBütce { get; set; }
        public decimal? ToplamBagis { get; set; }
        public ProjeDurumu Durum { get; set; } = ProjeDurumu.Planlandi;
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string? KapakFotoUrl { get; set; }

        public string OlusturanId { get; set; } = string.Empty;
        public virtual Uye? Olusturan { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // İlişkiler
        public virtual ICollection<ProjeBelge> Belgeler { get; set; } = [];
        public virtual ICollection<ProjeGuncelleme> Guncellemeler { get; set; } = [];

        // Hesaplanan
        public int FotografSayisi => Belgeler.Count(b => b.Tur == ProjeBelgeTuru.Fotograf);
        public int BelgeSayisi => Belgeler.Count(b => b.Tur == ProjeBelgeTuru.Belge);
        public int VideoSayisi => Belgeler.Count(b => b.Tur == ProjeBelgeTuru.Video);
    }

    public class ProjeBelge
    {
        public int Id { get; set; }
        public int ProjeId { get; set; }
        public virtual Proje? Proje { get; set; }

        public ProjeBelgeTuru Tur { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string DosyaYoluVeyaLink { get; set; } = string.Empty;  // Fotoğraf/belge = yol, Video = link
        public string? KucukResimUrl { get; set; }
        public long? DosyaBoyutu { get; set; }

        public string YukleyenId { get; set; } = string.Empty;
        public virtual Uye? Yukleyen { get; set; }
        public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
    }

    public class ProjeGuncelleme
    {
        public int Id { get; set; }
        public int ProjeId { get; set; }
        public virtual Proje? Proje { get; set; }

        public string Baslik { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public ProjeIlerleme IlerlemeYuzdesi { get; set; } = ProjeIlerleme.Yuzde0;

        public string YazanId { get; set; } = string.Empty;
        public virtual Uye? Yazan { get; set; }
        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }

    public enum ProjeKategori
    {
        SuKuyusu,
        OkulInsaati,
        CamiInsaati,
        GidaYardimi,
        Katarakt,
        Diger
    }

    public enum ProjeDurumu
    {
        Planlandi,
        Devam,
        Tamamlandi,
        Durduruldu
    }

    public enum ProjeBelgeTuru
    {
        Fotograf,
        Belge,
        Video
    }

    public enum ProjeIlerleme
    {
        Yuzde0 = 0,
        Yuzde10 = 10,
        Yuzde25 = 25,
        Yuzde50 = 50,
        Yuzde75 = 75,
        Yuzde90 = 90,
        Yuzde100 = 100
    }
}