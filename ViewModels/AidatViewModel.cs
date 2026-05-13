using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class TopluAidatViewModel
    {
        [Required(ErrorMessage = "Dönem zorunludur")]
        public string Donem { get; set; } = DateTime.UtcNow.ToString("yyyy-MM");

        [Required(ErrorMessage = "Tutar zorunludur")]
        [Range(1, 100000, ErrorMessage = "Geçerli bir tutar girin")]
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Son ödeme tarihi zorunludur")]
        public DateTime SonOdemeTarihi { get; set; } = DateTime.UtcNow.AddDays(30);
    }

    public class DekontYukleViewModel
    {
        public int AidatId { get; set; }
        public string Donem { get; set; } = string.Empty;
        public decimal Tutar { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi zorunludur")]
        public OdemeYontemi OdemeYontemi { get; set; }

        [Required(ErrorMessage = "Dekont zorunludur")]
        public IFormFile? Dekont { get; set; }
    }
}