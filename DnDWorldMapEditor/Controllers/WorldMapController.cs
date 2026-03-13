using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.Controllers
{
    public class WorldMapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public WorldMapController(ApplicationDbContext context,  IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: WorldMap
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorldMap.ToListAsync());
        }

        // GET: WorldMap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldMap = await _context.WorldMap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worldMap == null)
            {
                return NotFound();
            }

            return View(worldMap);
        }

        // GET: WorldMap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorldMap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [BindProperty]
        [Required]
        [Display(Name = "Background Image")]
        public IFormFile BackgroundImage { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,TotalRows,TotalColumns")] WorldMap worldMap)
        {
            ModelState.Remove("BackgroundImage");
            if (ModelState.IsValid)
            {
                if (User.Identity is { IsAuthenticated: true })
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    worldMap.UserId = userId;
                }
                else
                {
                    return RedirectToPage("/Identity/Account/Login/");
                }

                if (worldMap.TotalColumns * worldMap.TotalRows > Constants.MAX_GRID_SIZE)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                worldMap.BackgroundImage = BackgroundImage.FileName;
                
                var imageFile = Path.Combine(_environment.WebRootPath, "images", "worldMaps", BackgroundImage.FileName);
                using var fileStream = new FileStream(imageFile, FileMode.Create);
                await BackgroundImage.CopyToAsync(fileStream);
                
                _context.Add(worldMap);
                await _context.SaveChangesAsync();
                
                int worldMapId = worldMap.Id;
                for (int i = 0; i < worldMap.TotalRows; i++)
                {
                    for (int j = 0; j < worldMap.TotalColumns; j++)
                    {
                        GridSpace gridSpace = new GridSpace(worldMapId,  i, j);
                        _context.GridSpace.Add(gridSpace);
                        await _context.SaveChangesAsync();
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(worldMap);
        }

        
        // GET: WorldMap/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldMap = await _context.WorldMap.FindAsync(id);
            if (worldMap == null)
            {
                return NotFound();
            }
            return View(worldMap);
        }

        // POST: WorldMap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [BindProperty]
        [Display(Name = "Background Image")]
        public IFormFile? UpdatedImage { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,Description,TotalRows,TotalColumns")] WorldMap worldMap)
        {
            if (id != worldMap.Id)
            {
                return NotFound();
            }

            ModelState.Remove("BackgroundImage");
            if (ModelState.IsValid)
            {
                try
                {
                    if (UpdatedImage  != null)
                    {
                        var oldImage = Path.Combine(_environment.WebRootPath,"images", "worldMaps", worldMap.BackgroundImage);
                        if (!System.IO.File.Exists(oldImage)) System.IO.File.Delete(oldImage);
                        
                        worldMap.BackgroundImage = UpdatedImage.FileName;
                        var newImage = Path.Combine(_environment.WebRootPath, "images", "worldMaps", worldMap.BackgroundImage);
                        using var fileStream = new FileStream(newImage, FileMode.Create);
                        await UpdatedImage.CopyToAsync(fileStream);
                    }
                    _context.Update(worldMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorldMapExists(worldMap.Id))
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
            return View(worldMap);
        }

        // GET: WorldMap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldMap = await _context.WorldMap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worldMap == null)
            {
                return NotFound();
            }

            return View(worldMap);
        }

        // POST: WorldMap/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worldMap = await _context.WorldMap.FindAsync(id);
            if (worldMap != null)
            {
                var fileImage = Path.Combine(_environment.WebRootPath, "images", "worldMaps", worldMap.BackgroundImage);
                //ToDo Recursively delete gridSpaces associated with WorldMap
                int worldMapId = worldMap.Id;
                var gridSpaces = _context.GridSpace.OrderBy(x => x.Id).ToList();
                
                _context.WorldMap.Remove(worldMap);

                if (System.IO.File.Exists(fileImage))
                {
                    System.IO.File.Delete(fileImage);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorldMapExists(int id)
        {
            return _context.WorldMap.Any(e => e.Id == id);
        }
    }
}
