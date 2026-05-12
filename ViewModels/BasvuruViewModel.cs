using System.ComponentModel.DataAnnotations;

namespace DernekYonetim.ViewModels
{
    public class BasvuruViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        public string Telefon { get; set; } = string.Empty;

        public string? Adres { get; set; }
        public string? Meslek { get; set; }
        public string? BasvuruNotu { get; set; }
    }
}