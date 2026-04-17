using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DnDWorldMapEditor.Controllers
{
    public class GridSpaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<GridSpaceController> _logger;


        public GridSpaceController(ApplicationDbContext context, IWebHostEnvironment environment,
            ILogger<GridSpaceController> logger)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("History", "Description", "Notes", "Accessible")]
            GridSpaceEditDetailsViewModel updatedGridData)
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
        [Authorize]
        public async Task<IActionResult> AddCharacterToGridSpace(int gridSpaceId, int characterId)
        {
            _logger.LogInformation("Inside AddCharacterToGridSpace method - Character: {0}; GridSpace: {1}",
                characterId, gridSpaceId);
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            var character = await _context.Character.FindAsync(characterId);
            if (gridSpace == null || character == null)
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


                GridCharacter newGridCharacter = new GridCharacter(gridSpaceId, character.Id);
                _context.GridCharacter.Add(newGridCharacter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added Character: {1} to GridSpace: {0}", characterId, gridSpaceId);
                Tuple<GridCharacter, Character> viewData =
                    new Tuple<GridCharacter, Character>(newGridCharacter, character);

                return PartialView("GridCharacterDetails",viewData);
            }
            else
            {
                return RedirectToPage("/Identity/Account/Login/");
            }
        }


        [Authorize]
        public async Task<IActionResult> AddEncounterToGridSpace(int gridSpaceId, int encounterId)
        {
            _logger.LogInformation("Inside AddEncounterToGridSpace method - Encounter: {0}; GridSpace: {1}",
                encounterId, gridSpaceId);
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

                _logger.LogInformation("Added Encounter: {1} to GridSpace: {0}", encounterId, gridSpaceId);

                Tuple<GridEncounter, Encounter> viewData = new Tuple<GridEncounter, Encounter>(newGridEncounter, encounter );

                return PartialView("GridEncounterDetails", viewData);
            }
            else
            {
                return RedirectToPage("/Identity/Account/Login/");
            }
        }


        [Authorize]
        public async Task<IActionResult> GetGridSpaceDetailsById(int gridSpaceId)
        {
            GridSpace? gridSpace =
                await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null)
            {
                return BadRequest();
            }

            GridSpaceDetailsViewModel? viewModel = await GetGridSpaceDetailsViewModelData(gridSpace);
            if (viewModel == null)
            {
                return BadRequest();
            }

            return PartialView("GridSpaceDataModal", viewModel);
        }

        [Authorize]
        public async Task<IActionResult> GetGridSpaceDetails(int row, int col, int worldMapId)
        {
            GridSpace gridSpace =
                await _context.GridSpace.Where(x => x.Col == col && x.Row == row && x.WorldMapId == worldMapId)
                    .FirstAsync();

            GridSpaceDetailsViewModel? viewModel = await GetGridSpaceDetailsViewModelData(gridSpace);
            if (viewModel == null)
            {
                return NotFound();
            }

            return PartialView("GridSpaceDataModal", viewModel);
        }

        public async Task<GridSpaceDetailsViewModel?> GetGridSpaceDetailsViewModelData(GridSpace gridSpace)
        {
            List<GridEncounter> gridEncounters =
                await _context.GridEncounter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<GridCharacter> gridCharacters =
                await _context.GridCharacter.Where(x => x.GridSpaceId == gridSpace.Id).ToListAsync();
            List<Encounter> encounters = await _context.Encounter.ToListAsync();
            List<Character> characters = await _context.Character.ToListAsync();

            List<Tuple<GridEncounter, Encounter>> gridEncounterDetails = new List<Tuple<GridEncounter, Encounter>>();
            List<Tuple<GridCharacter, Character>> gridCharacterDetails = new List<Tuple<GridCharacter, Character>>();

            foreach (var gridEncounter in gridEncounters)
            {
                Encounter? encounter = encounters.Find(x => x.Id == gridEncounter.EncounterId);
                if (encounter == null)
                {
                    return null;
                }

                Tuple<GridEncounter, Encounter> encounterDetails =
                    new Tuple<GridEncounter, Encounter>(gridEncounter, encounter);
                gridEncounterDetails.Add(encounterDetails);
            }

            foreach (var gridCharacter in gridCharacters)
            {
                Character? character = characters.Find(x => x.Id == gridCharacter.CharacterId);
                if (character == null)
                {
                    return null;
                }

                Tuple<GridCharacter, Character> characterDetails =
                    new Tuple<GridCharacter, Character>(gridCharacter, character);
                gridCharacterDetails.Add(characterDetails);
            }

            GridSpaceDetailsViewModel viewModel = new GridSpaceDetailsViewModel()
            {
                GridSpace = gridSpace,
                GridEncounters = gridEncounterDetails,
                GridCharacters = gridCharacterDetails,
                Encounters = encounters,
                Characters = characters
            };

            return viewModel;
        }


        [Authorize]
        public async Task<IActionResult> UpdateDescription(int gridSpaceId, string updatedDescription)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null)
            {
                return NotFound();
            }

            gridSpace.Description = updatedDescription;

            _context.GridSpace.Update(gridSpace);
            await _context.SaveChangesAsync();

            return Ok(gridSpaceId);
        }

        public async Task<IActionResult> UpdateAccessibility(int gridSpaceId)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null)
            {
                return NotFound();
            }

            gridSpace.Accessible = !gridSpace.Accessible;

            _context.GridSpace.Update(gridSpace);
            await _context.SaveChangesAsync();

            return Ok(gridSpaceId);
        }

        [Authorize]
        public async Task<IActionResult> UpdateHistory(int gridSpaceId, string updatedHistory)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null)
            {
                return NotFound();
            }

            gridSpace.History = updatedHistory;

            _context.GridSpace.Update(gridSpace);
            await _context.SaveChangesAsync();

            return Ok(gridSpaceId);
        }

        [Authorize]
        public async Task<IActionResult> UpdateNotes(int gridSpaceId, string updatedNotes)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null)
            {
                return NotFound();
            }

            gridSpace.Notes = updatedNotes;

            _context.GridSpace.Update(gridSpace);
            await _context.SaveChangesAsync();

            return Ok(gridSpaceId);
        }

        [Authorize]
        public async Task<IActionResult> UpdateGridEncounterCompletedStatus(int gridEncounterId)
        {
            var gridEncounter = await _context.GridEncounter.FindAsync(gridEncounterId);
            var gridSpace = await _context.GridSpace.FindAsync(gridEncounter.GridSpaceId);
            if (gridEncounter == null || gridSpace == null)
            {
                return NotFound();
            }

            gridEncounter.IsCompleted = !gridEncounter.IsCompleted;
            _context.GridEncounter.Update(gridEncounter);
            await _context.SaveChangesAsync();

            return Ok(gridSpace.Id);
        }

        [Authorize]
        public async Task<IActionResult> RemoveGridEncounterFromGridSpace(int gridEncounterId)
        {
            var gridEncounter = await _context.GridEncounter.FindAsync(gridEncounterId);

            if (gridEncounter == null)
            {
                return NotFound();
            }

            int gridSpaceId = gridEncounter.GridSpaceId;

            _context.GridEncounter.Remove(gridEncounter);
            await _context.SaveChangesAsync();


            return Ok(gridSpaceId);
        }

        [Authorize]
        public async Task<IActionResult> RemoveGridCharacterFromGridSpace(int gridCharacterId)
        {
            var gridCharacter = await _context.GridCharacter.FindAsync(gridCharacterId);
            if (gridCharacter == null)
            {
                return NotFound();
            }

            int gridSpaceId = gridCharacter.GridSpaceId;

            _context.GridCharacter.Remove(gridCharacter);
            await _context.SaveChangesAsync();

            return Ok(gridSpaceId);
        }

        public async Task<IActionResult> GetGridSpaceDescriptionForm(int gridSpaceId)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null) return BadRequest();

            return PartialView("GridSpaceEditDescriptionModalForm", gridSpace);
        }

        public async Task<IActionResult> GetGridSpaceNotesForm(int gridSpaceId)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null) return BadRequest();

            return PartialView("GridSpaceEditNotesFormModal", gridSpace);
        }

        public async Task<IActionResult> GetGridSpaceHistoryForm(int gridSpaceId)
        {
            var gridSpace = await _context.GridSpace.FindAsync(gridSpaceId);
            if (gridSpace == null) return BadRequest();

            return PartialView("GridSpaceEditHistoryFormModal", gridSpace);
        }


        private bool GridSpaceExists(int id)
        {
            return _context.GridSpace.Any(e => e.Id == id);
        }
    }
}