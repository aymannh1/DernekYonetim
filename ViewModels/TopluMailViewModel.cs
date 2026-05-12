using System.ComponentModel.DataAnnotations;

namespace DernekYonetim.ViewModels
{
    public class TopluMailViewModel
    {
        [Required(ErrorMessage = "Konu zorunludur")]
        public string Konu { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        public string Hedef { get; set; } = "Tumu";  // Tumu, Aktif, Gecikti
    }
}