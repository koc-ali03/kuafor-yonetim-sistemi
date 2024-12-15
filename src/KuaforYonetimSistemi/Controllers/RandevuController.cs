using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KuaforYonetimSistemi.Data;
using KuaforYonetimSistemi.Models;

namespace KuaforYonetimSistemi.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Randevu/Create
        public IActionResult Create()
        {
            // Randevu oluşturulurken salonlar, çalışanlar ve servisler seçilsin
            ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Isim");
            ViewData["ServisId"] = new SelectList(_context.Servisler, "Id", "Isim");
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Isim");

            return View();
        }

        // POST: Randevu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RandevuTarihi,MusteriIsim,ServisId,CalisanId,SalonId")] Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                // Randevu kaydını veritabanına ekle
                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda dropdownları yeniden doldur
            ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Isim", randevu.SalonId);
            ViewData["ServisId"] = new SelectList(_context.Servisler, "Id", "Isim", randevu.ServisId);
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Isim", randevu.CalisanId);
            return View(randevu);
        }

        // GET: Randevu/Index
        public async Task<IActionResult> Index()
        {
            var randevular = _context.Randevus.Include(r => r.Servis).Include(r => r.Calisan).Include(r => r.Salon);
            return View(await randevular.ToListAsync());
        }
    }
}
