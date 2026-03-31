using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using Microsoft.IdentityModel.Tokens;

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
                
                return View(worldMap);
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

                
                string newFileName = GenerateUniqueFileName(worldMapVm.BackgroundImage.FileName);
                
                
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
                
                CreateGridSpaces(newWorldMap);
                
                return RedirectToAction(nameof(Index));
            }

            return BadRequest(ModelState); 
        }

        
        // // GET: WorldMap/Edit/5
        // //ToDo Create ViewModel for updating WorldMap Data, referencing _context model may be
        // //interfering with comparisons to old data for worldMap stored in the database. ViewModel might help to separate them as
        // //different objects for more accurate comparisons
        // [BindProperty]
        // public WorldMapCreateViewModel ViewModel { get; set; }
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var worldMap = await _context.WorldMap.FindAsync(id);
        //     ViewModel = new WorldMapViewModel()
        //     {
        //         Name = worldMap.Name,
        //         Description = worldMap.Description,
        //         MapSize = worldMap.MapSize,
        //         BackgroundImage = null,
        //         
        //     };
        //     
        //     if (worldMap == null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     
        //     return View(worldMap);
        // }

        // POST: WorldMap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [BindProperty]
        [Display(Name = "Background Image")]
        public IFormFile? UpdatedImage { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,Description,TotalRows,TotalColumns,BackgroundImage")] WorldMap worldMap)
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
                    if (UpdatedImage is not null)
                    {
                        string newFileName = GenerateUniqueFileName(UpdatedImage.FileName);
                        var oldPath = Path.Combine(_environment.WebRootPath,"images", "worldMaps", worldMap.BackgroundImage);
                        var newPath = Path.Combine(_environment.WebRootPath, "images", "worldMaps", newFileName);
                        worldMap.BackgroundImage = newFileName;

                        if (System.IO.File.Exists(oldPath))
                        {
                            ReplaceExistingImage(oldPath, newPath, UpdatedImage);
                        }
                    }
                    
                    var oldMapData = await _context.WorldMap.FindAsync(id);
                    Debug.Assert(oldMapData != null);
                    UpdateGridSpacesAfterMapEdit(oldMapData.TotalRows, oldMapData.TotalColumns, worldMap.TotalRows, worldMap.TotalColumns, id);

                    
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
                
                var gridSpaces = await _context.GridSpace.Where(x => x.WorldMapId == id).ToListAsync();
                DeleteGridSpaces(gridSpaces);
                
                _context.WorldMap.Remove(worldMap);

                if (System.IO.File.Exists(fileImage))
                {
                    System.IO.File.Delete(fileImage);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        
        //---------------------------------------------------
        //#HelperFunctions
        //---------------------------------------------------

        private bool WorldMapExists(int id)
        {
            return _context.WorldMap.Any(e => e.Id == id);
        }

        private string GenerateUniqueFileName(string file)
        {
            string ext = Path.GetExtension(file);
            string uniqueName = Guid.NewGuid().ToString();
            string uniqueFileName = uniqueName + ext;
            return uniqueFileName;
        }

        private async void UpdateGridSpacesAfterMapEdit(int oldRowsTotal, int oldColumnsTotal, int newRowsTotal, int newColumnsTotal, int worldMapId)
        {
            //ToDo Deleting gridSpaces when row/col count decreases doesn't work, FIX IT!
            //When lowering the number of rows, and col, an extra row is leftover
            int rowDiff = oldRowsTotal - newRowsTotal;
            int columnDiff = oldColumnsTotal - newColumnsTotal;
            bool rowIncrease = false;
            bool columnIncrease = false;
            bool rowDecrease = false;
            bool columnDecrease = false;
            var gridSpacesToDelete =  new List<GridSpace>();
            var gridSpacesToAdd =  new List<GridSpace>();

            if (rowDiff < 0) { rowIncrease = true; }
            else if (rowDiff > 0) { rowDecrease = true; }
            
            if (columnDiff < 0) { columnIncrease = true; }
            else if (columnDiff > 0) { columnDecrease = true; }
            

            //delete grid Spaces if col/rows decrease
            if (columnDecrease)
            {
                var colSpaces = await _context.GridSpace.Where(x => 
                    x.WorldMapId == worldMapId && 
                    x.Col > (newColumnsTotal - 1)).ToListAsync();
                foreach (var colSpace in colSpaces)
                {
                    gridSpacesToDelete.Add(colSpace);
                }
            }
            if (rowDecrease)
            {
                var rowSpaces = await _context.GridSpace.Where(x => 
                    x.WorldMapId == worldMapId && 
                    x.Row > (newRowsTotal - 1)).ToListAsync();
                foreach (var rowSpace in rowSpaces)
                {
                    gridSpacesToDelete.Add(rowSpace);
                }
            }

            if (gridSpacesToDelete.Count > 0)
            {
                DeleteGridSpaces(gridSpacesToDelete);   
            }

            //add grid Spaces if col/rows increase
            if (rowIncrease && columnIncrease)
            {
                for (int i = oldRowsTotal; i < newRowsTotal; i++)
                {
                    for (int j = 0; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
                for (int i = 0; i < oldRowsTotal; i++)
                {
                    for (int j = oldColumnsTotal; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                    }
                }
            }
            else if (rowIncrease && !columnIncrease)
            {
                for (int i = oldRowsTotal; i < newRowsTotal; i++)
                {
                    for (int j = 0; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
            }
            else if (columnIncrease && !rowIncrease)
            {
                for (int i = 0; i < newRowsTotal; i++)
                {
                    for (int j = oldColumnsTotal; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
            }

            if (gridSpacesToAdd.Count > 0)
            {
                AddGridSpaces(gridSpacesToAdd);
            }
            
            
        }

        public async void CreateGridSpaces(WorldMap worldMap)
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

        public async void AddGridSpaces(List<GridSpace> gridSpacesToAdd)
        {
            foreach (var gridSpace in gridSpacesToAdd)
            {
                _context.GridSpace.Add(gridSpace);
                await _context.SaveChangesAsync();
            }
        }
        
        public async void DeleteGridSpaces(List<GridSpace> gridSpacesToDelete)
        {
            foreach (var gridSpace in gridSpacesToDelete)
            {
                _context.GridSpace.Remove(gridSpace);
                await _context.SaveChangesAsync();
            } 
        }

        public async void ReplaceExistingImage(string oldImagePath, string newImagePath , IFormFile newImage)
        {
            System.IO.File.Delete(oldImagePath);
            using var fileStream = new FileStream(newImagePath, FileMode.Create);
            await newImage.CopyToAsync(fileStream);
        }
        
    }
}
