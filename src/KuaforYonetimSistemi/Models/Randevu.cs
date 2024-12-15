using System;
using System.ComponentModel.DataAnnotations;

namespace KuaforYonetimSistemi.Models
{
    public class Randevu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tarih ve saat zorunlu")]
        [Display(Name = "Randevu Tarihi ve Saati :")]
        public DateTime RandevuTarihi { get; set; }

        [Required(ErrorMessage = "Müşteri ismi zorunlu")]
        [Display(Name = "Müşteri Adı :")]
        public string MusteriIsim { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunlu")]
        [Display(Name = "Servis :")]
        public int ServisId { get; set; }
        public Servis Servis { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunlu")]
        [Display(Name = "Çalışan :")]
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        [Required(ErrorMessage = "Salon seçimi zorunlu")]
        [Display(Name = "Salon :")]
        public int SalonId { get; set; }
        public Salon Salon { get; set; }
    }
}
