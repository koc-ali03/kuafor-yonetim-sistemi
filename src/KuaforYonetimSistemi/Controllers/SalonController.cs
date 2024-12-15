using KuaforYonetimSistemi.Data;
using KuaforYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SalonController : Controller
{
    private readonly ApplicationDbContext _context;

    public SalonController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Tüm kullanıcılar (üyeler ve adminler) sadece listeleme yapabilir.
    public async Task<IActionResult> Index()
    {
        return View(await _context.Salons.ToListAsync());
    }

    // Tüm kullanıcılar (üyeler ve adminler) detayları görebilir.
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salon = await _context.Salons
            .FirstOrDefaultAsync(m => m.Id == id);
        if (salon == null)
        {
            return NotFound();
        }

        return View(salon);
    }

    // Sadece Admin rolündeki kullanıcılar oluşturabilir.
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Id,Isim,Adres,IletisimBilgisi,AcilisSaati,KapanisSaati")] Salon salon)
    {
        if (ModelState.IsValid)
        {
            _context.Add(salon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(salon);
    }

    // Sadece Admin rolündeki kullanıcılar düzenleyebilir.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salon = await _context.Salons.FindAsync(id);
        if (salon == null)
        {
            return NotFound();
        }
        return View(salon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Isim,Adres,IletisimBilgisi,AcilisSaati,KapanisSaati")] Salon salon)
    {
        if (id != salon.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(salon);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalonExists(salon.Id))
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
        return View(salon);
    }

    // Sadece Admin rolündeki kullanıcılar silebilir.
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salon = await _context.Salons
            .FirstOrDefaultAsync(m => m.Id == id);
        if (salon == null)
        {
            return NotFound();
        }

        return View(salon);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var salon = await _context.Salons.FindAsync(id);
        if (salon != null)
        {
            _context.Salons.Remove(salon);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SalonExists(int id)
    {
        return _context.Salons.Any(e => e.Id == id);
    }
}
