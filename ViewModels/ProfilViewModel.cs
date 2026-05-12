using System.ComponentModel.DataAnnotations;

namespace DernekYonetim.ViewModels
{
    public class ProfilViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        public string Telefon { get; set; } = string.Empty;

        public string? Meslek { get; set; }
        public string? Adres { get; set; }
    }
}