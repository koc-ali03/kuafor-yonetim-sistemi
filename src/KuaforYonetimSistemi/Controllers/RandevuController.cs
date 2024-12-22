using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KuaforYonetimSistemi.Data;
using KuaforYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace KuaforYonetimSistemi.Controllers
{
    [Authorize]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RandevuController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Randevu alma sayfası
        public IActionResult Randevu()
        {
            //ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Isim");
            //return View();
            if (!User.Identity.IsAuthenticated)
            {
                ViewData["IsAuthenticated"] = false;
            }
        else
            {
                ViewData["IsAuthenticated"] = true;
                ViewData["SalonId"] = new SelectList(_context.Salons, "Id", "Isim");
            }

            return View();
        }
        
        // Çalışanları salon tablosu üzerinden LINQ ile alır
        public JsonResult SalondanCalisanlariAl(int salonId)
        {
            var calisanlar = _context.Calisanlar
                                     .Where(c => c.SalonId == salonId)
                                     .Select(c => new { c.Id, c.Isim })
                                     .ToList();
            return Json(calisanlar);
        }

        // Servisleri çalışan tablosu üzerinden LINQ ile alır
        public JsonResult CalisandanServisleriAl(int calisanId)
        {
            var servisler = _context.Servisler
                                    .Where(s => s.CalisanId == calisanId)
                                    .Select(s => new { s.Id, s.Isim })
                                    .ToList();
            return Json(servisler);
        }

        // Çalışanların müsait olduğu saatleri alır
        public JsonResult RandevuSaatleriniAl(int calisanId, DateTime tarih)
        {
            var calisan = _context.Calisanlar.FirstOrDefault(c => c.Id == calisanId);

            if (calisan == null) return Json(new List<string>());

            var salon = _context.Salons.FirstOrDefault(s => s.Id == calisan.SalonId);
            if (salon == null) return Json(new List<string>());

            var baslangicSaat = salon.AcilisSaati;
            var bitisSaat = salon.KapanisSaati;

            var doluSaatler = _context.Randevular // Çalışanın o günkü randevularını alır
                                      .Where(r => r.Servis.CalisanId == calisanId && r.Tarih.Date == tarih.Date)
                                      .Select(r => r.BaslangicSaati)
                                      .ToList();

            var randevuSaatleri = new List<string>();
            for (var saat = baslangicSaat; saat < bitisSaat; saat = saat.Add(TimeSpan.FromHours(1)))
            {
                if (!doluSaatler.Contains(saat)) // Eğer dolu saatlerde değilse listeye ekler
                {
                    randevuSaatleri.Add(saat.ToString(@"hh\:mm"));
                }
            }

            return Json(randevuSaatleri);
        }

        // POST: Randevu alma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuAl(int ServisId, DateTime Tarih, TimeSpan BaslangicSaati)
        {
            // Girdi doğruluğunu kontrol eder
            if (ServisId == 0 || Tarih == DateTime.MinValue || BaslangicSaati == TimeSpan.Zero)
            {
                TempData["HataMesajı"] = "Lütfen tüm kutuları doldurun.";
                return RedirectToAction("Randevu");
            }

            // Seçilen servisi ve bağlı olduğu çalışanı alır
            var servis = _context.Servisler.Include(s => s.Calisan).ThenInclude(c => c.Salon).FirstOrDefault(s => s.Id == ServisId);

            if (servis == null || servis.Calisan == null || servis.Calisan.Salon == null)
            {
                TempData["HataMesajı"] = "Servis-Çalışan-Salon bağlantılarında hata var. Teknik destek ile görüşün.";
                return RedirectToAction("Randevu");
            }

            // Servisten çalışanı ve salonu alır (Çünkü direkt bağlantı yok)
            var calisan = servis.Calisan;
            var salon = calisan.Salon;

            // Seçilen saatin uygunluğunu kontrol eder (Çakışma kontrolü)
            var existingAppointment = _context.Randevular
                .FirstOrDefault(r => r.Servis.CalisanId == calisan.Id && r.Tarih.Date == Tarih.Date && r.BaslangicSaati == BaslangicSaati);

            if (existingAppointment != null)
            {
                TempData["HataMesajı"] = "Bu saat için çoktan randevu alınmış, Lütfen farklı bir saat seçin.";
                return RedirectToAction("Randevu");
            }

            // Yeni randevu nesnesi oluşturulur (Veritabanına ekleyebilmek için)
            var randevu = new Randevu
            {
                Tarih = Tarih + BaslangicSaati,
                BaslangicSaati = BaslangicSaati,
                KullaniciId = _userManager.GetUserId(User),  // Assuming the logged-in user's ID is used for the appointment
                Onaylandi = false // Assuming the appointment is not confirmed yet
            };

            // Seçilen servis randevuya eklenir
            randevu.ServisId = servis.Id;

            // Oluşturulan randevu nesnesi _context'e eklenir (Veritabanına kayıt)
            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            // Kullanıcı farklı sayfaya yönlendirilir
            return RedirectToAction("Index");
        }


        // GET: Randevu
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);

            // Kullanıcı admin rolüne sahip mi diye kontrol eder
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            IQueryable<Randevu> randevular;

            if (isAdmin)
            {
                // Admin ise tüm randevuları gösterir
                randevular = _context.Randevular
                    .Include(r => r.Servis)
                    .Include(r => r.Servis.Calisan)
                    .Include(r => r.Servis.Calisan.Salon)
                    .Include(r => r.Kullanici);
            }
            else
            {
                // Sıradan bir kullanıcı ise sadece kendisine ait randevuları gösterir
                randevular = _context.Randevular
                    .Include(r => r.Servis)
                    .Include(r => r.Servis.Calisan)
                    .Include(r => r.Servis.Calisan.Salon)
                    .Where(r => r.KullaniciId == currentUserId);
            }

            return View(await randevular.ToListAsync());
        }

        // GET: Randevu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Servis.Calisan)
                .Include(r => r.Servis)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // GET: Randevu/Create
        public IActionResult Create()
        {
            ViewData["KullaniciId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["ServisId"] = new SelectList(
                 _context.Servisler.Include(s => s.Calisan)
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       DisplayName = $"{s.Isim} ({s.Calisan.Isim})"
                                   }),
                "Id", "DisplayName"
                );
            return View();
        }

        // POST: Randevu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tarih,BaslangicSaati,ServisId,KullaniciId,Onaylandi")] Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                randevu.BaslangicSaati = randevu.Tarih.TimeOfDay;

                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Users, "Id", "UserName", randevu.KullaniciId);
            ViewData["ServisId"] = new SelectList(
             _context.Servisler.Include(s => s.Calisan)
                               .Select(s => new
                               {
                                   Id = s.Id,
                                   DisplayName = $"{s.Isim} ({s.Calisan.Isim})"
                               }),
            "Id", "DisplayName"
            );
            return View(randevu);
        }

        // GET: Randevu/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }
            ViewData["KullaniciId"] = new SelectList(_context.Users, "Id", "UserName", randevu.KullaniciId);
            ViewData["ServisId"] = new SelectList(
             _context.Servisler.Include(s => s.Calisan)
                               .Select(s => new
                               {
                                   Id = s.Id,
                                   DisplayName = $"{s.Isim} ({s.Calisan.Isim})"
                               }),
            "Id", "DisplayName"
            );
            return View(randevu);
        }

        // POST: Randevu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tarih,BaslangicSaati,ServisId,KullaniciId,Onaylandi")] Randevu randevu)
        {
            if (id != randevu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    randevu.BaslangicSaati = randevu.Tarih.TimeOfDay;

                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RandevuExists(randevu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Users, "Id", "UserName", randevu.KullaniciId);
            ViewData["ServisId"] = new SelectList(
             _context.Servisler.Include(s => s.Calisan)
                               .Select(s => new
                               {
                                   Id = s.Id,
                                   DisplayName = $"{s.Isim} ({s.Calisan.Isim})"
                               }),
            "Id", "DisplayName"
            );
            return View(randevu);
        }

        // GET: Randevu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Servis.Calisan)
                .Include(r => r.Servis)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);

            // Kullanıcı admin mi
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Kullanıcı admin mi, ya da giriş yapan kullanıcının randevuyu alan kullanıcı ile aynı mı kontrol eder
            if (!(randevu.KullaniciId == currentUserId || isAdmin))
            {
                // Eğer başka bir kullanıcının randevusu silinmeye çalışılıyorsa, hata mesajı gösterir
                return Forbid();
            }

            return View(randevu);
        }

        // POST: Randevu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu == null)
            {
                return NotFound();
            }

            // Giriş yapan kullanıcının randevuyu alan kullanıcı ile aynı olup olmadığını kontrol eder
            var currentUserId = _userManager.GetUserId(User);
            if (randevu.KullaniciId != currentUserId)
            {
                // Eğer başka bir kullanıcının randevusu silinmeye çalışılıyorsa, hata mesajı gösterir
                return Forbid();
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}
