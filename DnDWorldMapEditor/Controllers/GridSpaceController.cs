using System.Security.Claims;
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
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<GridSpaceController> _logger;
        

        public GridSpaceController(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<GridSpaceController> logger)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("History", "Description", "Notes", "Accessible")] GridSpaceEditDetailsViewModel updatedGridData)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            if (gridSpace == null)
            {
                return NotFound();
            }

            gridSpace.History = updatedGridData.History;
            gridSpace.Description = updatedGridData.Description;
            gridSpace.Notes = updatedGridData.Notes;
            gridSpace.Accessible = updatedGridData.Accessible;

            _context.GridSpace.Update(gridSpace);
            await _context.SaveChangesAsync();

            return await GetGridSpaceDetails(id, 3, 3);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacterToGridSpace(int id, Character character)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            if (gridSpace == null)
            {
                return NotFound();
            }

            GridCharacter newGridCharacter = new GridCharacter(id, character.Id);
            _context.GridCharacter.Add(newGridCharacter);
            await _context.SaveChangesAsync();

            return await GetGridSpaceDetails(id, 3, 3);
        }

        [HttpPost]
        public async Task<IActionResult> AddEncounterToGridSpace(int gridSpaceId, int encounterId)
        {
            _logger.LogInformation("Inside AddEncounterToGridSpace method - Encounter: {0}; GridSpace: {1}",encounterId, gridSpaceId);
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            var encounter = await _context.Encounter.FindAsync(encounterId);
            if (gridSpace == null || encounter == null)
            {
                return NotFound();
            }
            
            
            if (User.Identity is { IsAuthenticated: true })
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToPage("/Identity/Account/Login/");
                }
                
                
                GridEncounter newGridEncounter = new GridEncounter(gridSpaceId, encounter.Id, false);
                _context.GridEncounter.Add(newGridEncounter);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Added Encounter: {1} to GridSpace: {0}",encounterId, gridSpaceId);

                return Ok();
            }
            else
            {
                return RedirectToPage("/Identity/Account/Login/");
            }
            
        }

        public async Task<IActionResult> GetGridSpaceDetails(int row, int col, int worldMapId)
        {
            GridSpace gridSpace =
                await _context.GridSpace.Where(x => x.Col == col && x.Row == row && x.WorldMapId == worldMapId)
                    .FirstAsync();

            List<GridEncounter> gridEncounters =
                await _context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<GridCharacter> gridCharacters =
                await _context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<Encounter> encounters = await _context.Encounter.ToListAsync();
            List<Character> characters = await _context.Character.ToListAsync();

            GridSpaceDetailsViewModel viewModel = new GridSpaceDetailsViewModel()
            {
                GridSpace = gridSpace,
                GridEncounters = gridEncounters,
                GridCharacters = gridCharacters,
                Encounters = encounters,
                Characters = characters
            };


            return PartialView("GridSpaceDataModal", viewModel);
        }
        
        
        
        private bool GridSpaceExists(int id)
        {
            return _context.GridSpace.Any(e => e.Id == id);
        }
    }
}