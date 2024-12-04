using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KuaforSalonu.Models
{
    public class Salon
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Isim zorunlu")]
        [Display(Name="İsim :")]
        public string Isim { get; set; }

        [Required(ErrorMessage = "Adres zorunlu")]
        [Display(Name = "Adres :")]
        public string Adres { get; set; }

        [Required(ErrorMessage = "İletişim bilgisi zorunlu")]
        [Display(Name = "İletişim Bilgisi :")]
        public string IletisimBilgisi { get; set; }

        [Required(ErrorMessage = "Çalışma saatleri zorunlu")]
        [Display(Name = "Çalışma Saatleri :")]
        public string CalismaSaatleri { get; set; }

        public ICollection<Calisan> Calisanlar { get; set; } = new List<Calisan>();
    }
}
