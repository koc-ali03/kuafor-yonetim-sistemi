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

namespace KuaforYonetimSistemi.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Randevu
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Randevular.Include(r => r.Kullanici).Include(r => r.Servis).Include(r => r.Servis.Calisan);
            return View(await applicationDbContext.ToListAsync());
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

            return View(randevu);
        }

        // POST: Randevu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}
