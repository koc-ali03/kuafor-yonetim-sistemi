using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforYonetimSistemi.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KuaforYonetimSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPaneliApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminPaneliApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminPaneliApi/EnPopulerServisler
        [HttpGet("EnPopulerServisler")]
        public IActionResult EnPopulerServisler()
        {
            var enPopulerServisler = _context.Randevular
                .GroupBy(r => r.ServisId)
                .Select(g => new
                {
                    ServisAdi = _context.Servisler.FirstOrDefault(s => s.Id == g.Key).Isim,
                    CalisanAdi = _context.Servisler.FirstOrDefault(s => s.Id == g.Key).Calisan.Isim,
                    Sayi = g.Count()
                })
                .OrderByDescending(s => s.Sayi)
                .ToList();

            return Ok(enPopulerServisler);
        }

        // GET: api/AdminApi/GunlukRandevuSayisi
        [HttpGet("GunlukRandevuSayisi")]
        public IActionResult GunlukRandevuSayisi()
        {
            var sonYediGun = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-i))
                .ToList();

            var gunlukRandevular = sonYediGun
                .Select(gun => new
                {
                    Tarih = gun.Day + "." + gun.Month + "." + gun.Year,
                    Sayi = _context.Randevular.Count(r => r.Tarih.Date == gun)
                })
                .OrderBy(d => d.Tarih)
                .ToList();

            return Ok(gunlukRandevular);
        }

        // GET: api/AdminPaneliApi/OnaylanmamisRandevular
        [HttpGet("OnaylanmamisRandevular")]
        public IActionResult OnaylanmamisRandevular()
        {
            var onaylanmamisRandevular = _context.Randevular
                .Where(r => !r.Onaylandi)
                .Select(r => new
                {
                    Id = r.Id,
                    Tarih = r.Tarih,
                    Isim = r.Servis.Isim,
                    KullaniciAdi = r.Kullanici.UserName,
                    CalisanAdi = r.Servis.Calisan.Isim
                })
                .ToList();

            return Ok(onaylanmamisRandevular);
        }

        // POST: api/AdminPaneliApi/RandevuOnayla/{id}
        [HttpPost("RandevuOnayla/{id}")]
        public IActionResult RandevuOnayla(int id)
        {
            var randevu = _context.Randevular.FirstOrDefault(r => r.Id == id);

            if (randevu == null)
                return NotFound("Randevu bulunamadı");

            randevu.Onaylandi = true;
            _context.SaveChanges();

            return Ok($"Randevu ID: {id} onaylandı");
        }

        // POST: api/AdminPaneliApi/RandevuSil/{id}
        [HttpPost("RandevuSil/{id}")]
        public IActionResult RandevuSil(int id)
        {
            var randevu = _context.Randevular.FirstOrDefault(r => r.Id == id);

            if (randevu == null)
                return NotFound("Randevu bulunamadı");

            _context.Randevular.Remove(randevu);
            _context.SaveChanges();

            return Ok($"Randevu ID: {id} silindi");
        }

    }
}
