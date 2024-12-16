using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KuaforYonetimSistemi.Models
{
    public class Randevu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunlu")]
        [Display(Name = "Randevu Tarihi :")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunlu")]
        [Display(Name = "Başlangıç Saati :")]
        public TimeSpan BaslangicSaati { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunlu")]
        [Display(Name = "Servis :")]
        public int ServisId { get; set; }

        public Servis? Servis { get; set; }

        [Required(ErrorMessage = "Kullanıcı seçimi zorunlu")]
        [Display(Name = "Kullanıcı :")]
        public string KullaniciId { get; set; }

        public IdentityUser? Kullanici { get; set; }

        [Required]
        [Display(Name = "Onaylandı mı :")]
        public bool Onaylandi { get; set; } = false;
    }
}
