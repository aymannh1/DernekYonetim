using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class MaliHareketViewModel
    {
        [Required(ErrorMessage = "Tür zorunludur")]
        public MaliHareketTuru Tur { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur")]
        public string Kategori { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        public string Aciklama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tutar zorunludur")]
        [Range(0.01, 10000000, ErrorMessage = "Geçerli bir tutar girin")]
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur")]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;
    }
}