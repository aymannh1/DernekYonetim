using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class EtkinlikViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur")]
        public EtkinlikKategori Kategori { get; set; }

        public string? Yer { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Bitiş tarihi zorunludur")]
        public DateTime BitisTarihi { get; set; } = DateTime.UtcNow.AddHours(2);

        public int? Kontenjan { get; set; }
        public bool UcretsizMi { get; set; } = true;
        public decimal? Ucret { get; set; }
        public EtkinlikDurumu Durum { get; set; } = EtkinlikDurumu.Taslak;
    }
}