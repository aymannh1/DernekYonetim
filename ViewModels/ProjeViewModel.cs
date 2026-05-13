using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class ProjeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur")]
        public ProjeKategori Kategori { get; set; }

        public string? DigerKategori { get; set; }

        [Required(ErrorMessage = "Ülke zorunludur")]
        public string Ulke { get; set; } = string.Empty;

        public string? Sehir { get; set; }

        public decimal? HedefButce { get; set; }
        public decimal? ToplamBagis { get; set; }

        public ProjeDurumu Durum { get; set; } = ProjeDurumu.Planlandi;

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? BitisTarihi { get; set; }
        public IFormFile? KapakFoto { get; set; }
    }

    public class ProjeBelgeYukleViewModel
    {
        public int ProjeId { get; set; }

        [Required(ErrorMessage = "Tür zorunludur")]
        public ProjeBelgeTuru Tur { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        // Fotoğraf / Belge için
        public IFormFile? Dosya { get; set; }

        // Video için
        public string? VideoLink { get; set; }
    }

    public class ProjeGuncellemeViewModel
    {
        public int ProjeId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        public ProjeIlerleme IlerlemeYuzdesi { get; set; } = ProjeIlerleme.Yuzde0;
    }
}