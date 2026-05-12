using System.ComponentModel.DataAnnotations;

namespace DernekYonetim.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        public bool BeniHatirla { get; set; }
    }
}