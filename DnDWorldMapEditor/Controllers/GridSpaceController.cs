using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridSpaceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GridSpaceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GridSpace
        [HttpGet]
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

        // PUT: api/GridSpace/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGridSpace(int id, GridSpace gridSpace)
        {
            if (id != gridSpace.Id)
            {
                return BadRequest();
            }

            _context.Entry(gridSpace).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GridSpaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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

        // DELETE: api/GridSpace/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGridSpace(int id)
        {
            var gridSpace = await _context.GridSpace.FindAsync(id);
            if (gridSpace == null)
            {
                return NotFound();
            }

            _context.GridSpace.Remove(gridSpace);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GridSpaceExists(int id)
        {
            return _context.GridSpace.Any(e => e.Id == id);
        }
    }
}
