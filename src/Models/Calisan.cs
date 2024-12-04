using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KuaforSalonu.Models
{
    public class Calisan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Isim zorunlu")]
        [Display(Name = "İsim :")]
        public string Isim { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunlu")]
        [Display(Name = "Uzmanlık Alanı :")]
        public string UzmanlikAlani { get; set; }

        [Required(ErrorMessage = "Salon seçimi zorunlu")]
        [Display(Name = "Salon Seçimi :")]
        public int SalonId { get; set; }

        public Salon? Salon { get; set; }
        public ICollection<Servis> Servisler { get; set; } = new List<Servis>();
    }
}
