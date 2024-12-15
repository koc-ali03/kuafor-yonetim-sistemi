using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KuaforYonetimSistemi.Models
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

        [Required(ErrorMessage = "Açılış saati zorunlu")]
        [Display(Name = "Açılış Saati :")]
        public TimeSpan AcilisSaati { get; set; }

        [Required(ErrorMessage = "Kapanış saati zorunlu")]
        [Display(Name = "Kapanış Saati :")]
        public TimeSpan KapanisSaati { get; set; }

        public ICollection<Calisan> Calisanlar { get; set; } = new List<Calisan>();
    }
}
