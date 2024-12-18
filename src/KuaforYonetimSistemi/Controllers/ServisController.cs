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
    [Authorize(Roles = "Admin")]
    public class ServisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servis
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Servisler.Include(s => s.Calisan);
            return View(await ApplicationDbContext.ToListAsync());
        }

        // GET: Servis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servis = await _context.Servisler
                .Include(s => s.Calisan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servis == null)
            {
                return NotFound();
            }

            return View(servis);
        }

        // GET: Servis/Create
        public IActionResult Create()
        {
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Isim");
            return View();
        }

        // POST: Servis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Isim,Sure,Ucret,CalisanId")] Servis servis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Isim", servis.CalisanId);
            return View(servis);
        }

        // GET: Servis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servis = await _context.Servisler.FindAsync(id);
            if (servis == null)
            {
                return NotFound();
            }
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Id", servis.CalisanId);
            return View(servis);
        }

        // POST: Servis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Isim,Sure,Ucret,CalisanId")] Servis servis)
        {
            if (id != servis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServisExists(servis.Id))
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
            ViewData["CalisanId"] = new SelectList(_context.Calisanlar, "Id", "Id", servis.CalisanId);
            return View(servis);
        }

        // GET: Servis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servis = await _context.Servisler
                .Include(s => s.Calisan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servis == null)
            {
                return NotFound();
            }

            return View(servis);
        }

        // POST: Servis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servis = await _context.Servisler.FindAsync(id);
            if (servis != null)
            {
                _context.Servisler.Remove(servis);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServisExists(int id)
        {
            return _context.Servisler.Any(e => e.Id == id);
        }
    }
}
