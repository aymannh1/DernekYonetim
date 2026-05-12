using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class BelgeYukleViewModel
    {
        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur")]
        public string Kategori { get; set; } = string.Empty;

        public BelgeErisim ErisimSeviyesi { get; set; } = BelgeErisim.UyeyeOzel;

        [Required(ErrorMessage = "Dosya zorunludur")]
        public IFormFile? Dosya { get; set; }
    }
}