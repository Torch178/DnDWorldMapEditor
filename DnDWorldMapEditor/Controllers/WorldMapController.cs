
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using Microsoft.IdentityModel.Tokens;
using DnDWorldMapEditor.HelperFunctions;

namespace DnDWorldMapEditor.Controllers
{
    public class WorldMapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<WorldMapController> _logger;
        
        public WorldMapController(ApplicationDbContext context,  IWebHostEnvironment environment, ILogger<WorldMapController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: WorldMap
        public async Task<IActionResult> Index()
        {
            try
            {
                var worldMaps = await _context.WorldMap.ToListAsync();
                _logger.LogInformation("World Map Index Called, World Map Count: {worldMaps.Count}", worldMaps.Count);
                return View(worldMaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return NotFound();
        }

        // GET: WorldMap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                _logger.LogInformation("Fetching WorldMap ");
                if (id == null)
                {
                    _logger.LogError("WorldMap Details not found, id is null\nId: {id}", id);
                    return NotFound();
                }

                var worldMap = await _context.WorldMap
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (worldMap == null)
                {
                    _logger.LogError("WorldMap Details not found, id is null\nWorldMap: {wm}", worldMap);
                    return NotFound();
                }

                var gridSpaces = _context.GridSpace.Where(x => x.WorldMapId == worldMap.Id).ToList();
                List<GridEncounter> gridEncounters = new List<GridEncounter>();
                List<GridCharacter> gridCharacters = new List<GridCharacter>();
                foreach (var gridSpace in gridSpaces)
                {
                    gridEncounters.AddRange(_context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToList());
                    gridCharacters.AddRange(_context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToList());
                }
                WorldMapDetailsViewModel viewModel = new WorldMapDetailsViewModel()
                {
                    WorldMap = worldMap,
                    GridSpaces = gridSpaces,
                    GridCharacters = gridCharacters,
                    GridEncounters = gridEncounters,

                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Caught in WorldMap Details Get -> id:{id}\n{ex}",id ,ex.Message);
            }
            return NotFound();

        }

        // GET: WorldMap/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: WorldMap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,MapSize,BackgroundImage")] WorldMapCreateViewModel worldMapVm)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Creating WorldMap input: {0}", worldMapVm);
                WorldMap newWorldMap = new WorldMap
                {
                    
                    Name = worldMapVm.Name,
                    Description = worldMapVm.Description,
                    TotalRows = 5,
                    TotalColumns = 5,
                    MapSize = worldMapVm.MapSize,
                    BackgroundImage = string.Empty,
                };

                if (worldMapVm.BackgroundImage.Length == 0)
                {
                    return BadRequest("World Map Background Image cannot be an empty file");
                }

                if (worldMapVm.Name.IsNullOrEmpty())
                {
                    return BadRequest("World Map must have a name.");
                }
                
                if (User.Identity is { IsAuthenticated: true })
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    newWorldMap.UserId = userId;
                }
                else
                {
                    return RedirectToPage("/Identity/Account/Login/");
                }

                string newFileName = FileFunctions.GenerateUniqueFileName(worldMapVm.BackgroundImage.FileName);
                var imageFile = Path.Combine(_environment.WebRootPath, "images", "worldMaps", newFileName);
                await using var fileStream = new FileStream(imageFile, FileMode.Create);
                await worldMapVm.BackgroundImage.CopyToAsync(fileStream);
                newWorldMap.BackgroundImage = newFileName;

                switch (newWorldMap.MapSize)
                {
                    case "Small":
                        newWorldMap.TotalRows = 5;
                        newWorldMap.TotalColumns = 5;
                        break;
                    case "Medium":
                        newWorldMap.TotalRows = 7;
                        newWorldMap.TotalColumns = 7;
                        break;
                    case "Large":
                        newWorldMap.TotalRows = 10;
                        newWorldMap.TotalColumns = 10;
                        break;
                    default:
                        return BadRequest("Invalid Map Size Format. Map Size must be Small, Medium, or Large.");
                }
                
                _context.Add(newWorldMap);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created WorldMap: {0}", newWorldMap);
                await CreateGridSpaces( newWorldMap);
                int gridSpaceCount = await _context.GridSpace.CountAsync(x => x.WorldMapId == newWorldMap.Id);
                _logger.LogInformation("Created GridSpaces, Count: {0} for WorldMap: {1}", gridSpaceCount , newWorldMap.Id);
                return RedirectToAction(nameof(Index));
            }

            return BadRequest(ModelState); 
        }

        
        // // GET: WorldMap/Edit/5
        [BindProperty]
        private WorldMapEditViewModel ViewModel { get; set; }
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
            
            ViewModel = new WorldMapEditViewModel()
            {
                Name = worldMap.Name,
                Description = worldMap.Description,
                OldImage = worldMap.BackgroundImage,
                NewImage = null,
            };
            
            return View(ViewModel);
        }

        // POST: WorldMap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,OldImage,NewImage")] WorldMapEditViewModel updatedMap)
        {
            WorldMap? worldMap = _context.WorldMap.FirstOrDefault(m => m.Id == id);
            if (worldMap is null)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (updatedMap.NewImage is not null )
                    {
                        if(updatedMap.NewImage.Length != 0)
                        {
                            string newFileName = FileFunctions.GenerateUniqueFileName(updatedMap.NewImage.FileName);
                            var oldPath = Path.Combine(_environment.WebRootPath,"images", "worldMaps", worldMap.BackgroundImage);
                            var newPath = Path.Combine(_environment.WebRootPath, "images", "worldMaps", newFileName);
                            worldMap.BackgroundImage = newFileName;
                            if (System.IO.File.Exists(oldPath))
                            {
                                FileFunctions.ReplaceExistingImage(oldPath, newPath, updatedMap.NewImage);
                            }
                        }
                        else
                        {
                            return BadRequest("Error: New Background Image uploaded is empty.");
                        }
                    }
                    
                    worldMap.Name = updatedMap.Name;
                    worldMap.Description = updatedMap.Description;
                    
                    _context.Update(worldMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, ex.Message);
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
                
                var gridSpaces = await _context.GridSpace.Where(x => x.WorldMapId == id).ToListAsync();
                await DeleteGridSpaces(gridSpaces);
                
                _context.WorldMap.Remove(worldMap);

                if (System.IO.File.Exists(fileImage))
                {
                    System.IO.File.Delete(fileImage);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task CreateGridSpaces(WorldMap worldMap)
        {
            
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

        }
        
        public async Task DeleteGridSpaces(List<GridSpace> gridSpacesToDelete)
        {
            foreach (var gridSpace in gridSpacesToDelete)
            {
                _context.GridSpace.Remove(gridSpace);
                await _context.SaveChangesAsync();
            } 
        }

    }
     
}
