using System.ComponentModel.DataAnnotations;
using DernekYonetim.Models;

namespace DernekYonetim.ViewModels
{
    public class DuyuruViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;

        public string? Ozet { get; set; }
        public DuyuruHedef Hedef { get; set; } = DuyuruHedef.TumUyeler;
        public bool KamuoyunaAcik { get; set; } = false;
        public DateTime? YayinTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool Aktif { get; set; } = true;
    }
}