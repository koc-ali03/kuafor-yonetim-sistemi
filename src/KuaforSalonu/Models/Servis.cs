using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KuaforSalonu.Models
{
    public class Servis
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Isim zorunlu")]
        [Display(Name = "İsim :")]
        public string Isim { get; set; }

        [Required(ErrorMessage = "Süre zorunlu")]
        [Display(Name = "Süre :")]
        public int Sure { get; set; }

        [Required(ErrorMessage = "Ücret zorunlu")]
        [Display(Name = "Ücret :")]
        public decimal Ucret { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunlu")]
        [Display(Name = "Çalışan seçimi :")]
        public int CalisanId { get; set; }

        public Calisan? Calisan { get; set; }
    }
}
