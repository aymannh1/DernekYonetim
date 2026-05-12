using System.ComponentModel.DataAnnotations;

namespace DernekYonetim.ViewModels
{
    public class SifreDegistirViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunludur")]
        [DataType(DataType.Password)]
        public string MevcutSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [MinLength(8, ErrorMessage = "En az 8 karakter olmalı")]
        [DataType(DataType.Password)]
        public string YeniSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor")]
        [DataType(DataType.Password)]
        public string YeniSifreTekrar { get; set; } = string.Empty;
    }
}