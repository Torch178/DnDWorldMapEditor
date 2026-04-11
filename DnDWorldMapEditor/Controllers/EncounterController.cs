using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DnDWorldMapEditor.Controllers
{
    public class EncounterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EncounterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Encounter
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Encounter.Where(x => x.UserId == userId).ToListAsync());
        }

        // GET: Encounter/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encounter = await _context.Encounter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (encounter == null)
            {
                return NotFound();
            }

            return View(encounter);
        }

        // GET: Encounter/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Encounter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Encounter encounter)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity is { IsAuthenticated: true })
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    encounter.UserId = userId;
                }
                else
                {
                    return RedirectToPage("/Identity/Account/Login/");
                }

                _context.Add(encounter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(encounter);
        }

        // GET: Encounter/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encounter = await _context.Encounter.FindAsync(id);
            if (encounter == null)
            {
                return NotFound();
            }

            return View(encounter);
        }

        // POST: Encounter/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Encounter encounter)
        {
            if (id != encounter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(encounter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncounterExists(encounter.Id))
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

            return View(encounter);
        }

        // GET: Encounter/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encounter = await _context.Encounter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (encounter == null)
            {
                return NotFound();
            }

            return View(encounter);
        }

        // POST: Encounter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var encounter = await _context.Encounter.FindAsync(id);
            if (encounter != null)
            {
                _context.Encounter.Remove(encounter);
            }

            var gridEncounters = await _context.GridEncounter.Where(x => x.EncounterId == id).ToListAsync();
            _context.RemoveRange(gridEncounters);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EncounterExists(int id)
        {
            return _context.Encounter.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> GetEncounter(int encounterId)
        {
            Encounter? encounter = await _context.Encounter.FindAsync(encounterId);

            if (encounter == null) return NotFound();

            return PartialView("EncounterCard", encounter);
        }
    }
}