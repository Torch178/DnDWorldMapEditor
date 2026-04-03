using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;

namespace DnDWorldMapEditor.Controllers
{
    
    public class GridSpaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GridSpaceController> _logger;

        public GridSpaceController(ApplicationDbContext context, ILogger<GridSpaceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/GridSpace
        public async Task<ActionResult<IEnumerable<GridSpace>>> GetGridSpace()
        {
            return await _context.GridSpace.ToListAsync();
        }

        // GET: api/GridSpace/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GridSpace>> GetGridSpace(int id)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);

            if (gridSpace == null)
            {
                return NotFound();
            }

            return gridSpace;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("History", "Description", "Notes", "Accessible")] GridSpaceEditDetailsViewModel updatedGridData)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            List<GridEncounter> gridEncounters = await _context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<GridCharacter> gridCharacters = await _context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            
            GridSpaceDetailsViewModel model = new GridSpaceDetailsViewModel()
            {
                gridSpace = gridSpace,
                gridCharacters = gridCharacters,
                gridEncounters = gridEncounters

            };

            return PartialView("GridSpaceDataModal", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacterToGridSpace(int id, Character character)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            List<GridEncounter> gridEncounters = await _context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<GridCharacter> gridCharacters = await _context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            
            GridSpaceDetailsViewModel model = new GridSpaceDetailsViewModel()
            {
                gridSpace = gridSpace,
                gridCharacters = gridCharacters,
                gridEncounters = gridEncounters

            };

            return PartialView("GridSpaceDataModal", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEncounterToGridSpace(int id, Encounter encounter)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            List<GridEncounter> gridEncounters = await _context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<GridCharacter> gridCharacters = await _context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            
            GridSpaceDetailsViewModel model = new GridSpaceDetailsViewModel()
            {
                gridSpace = gridSpace,
                gridCharacters = gridCharacters,
                gridEncounters = gridEncounters
            };

            return PartialView("GridSpaceDataModal", model);
        }

        // POST: api/GridSpace
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GridSpace>> PostGridSpace(GridSpace gridSpace)
        {
            _context.GridSpace.Add(gridSpace);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGridSpace", new { id = gridSpace.Id }, gridSpace);
        }
        

        private bool GridSpaceExists(int id)
        {
            return _context.GridSpace.Any(e => e.Id == id);
        }
    }
}
