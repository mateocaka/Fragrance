using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fragrance.Data;
using Fragrance.Models;

namespace Fragrance.Areas.Admin.Controllers
{
    [Route("api/ParfumeAPI")]
    [ApiController]
    public class ParfumeAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParfumeAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ParfumeAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parfume>>> GetParfumes()
        {
            return await _context.Parfumes.ToListAsync();
        }

        // GET: api/ParfumeAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parfume>> GetParfume(int id)
        {
            var parfume = await _context.Parfumes.FindAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            return parfume;
        }

        // PUT: api/ParfumeAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParfume(int id, Parfume parfume)
        {
            if (id != parfume.ParfumeId)
            {
                return BadRequest();
            }

            _context.Entry(parfume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParfumeExists(id))
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

        // POST: api/ParfumeAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Parfume>> PostParfume(Parfume parfume)
        {
            _context.Parfumes.Add(parfume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParfume", new { id = parfume.ParfumeId }, parfume);
        }

        // DELETE: api/ParfumeAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParfume(int id)
        {
            var parfume = await _context.Parfumes.FindAsync(id);
            if (parfume == null)
            {
                return NotFound();
            }

            _context.Parfumes.Remove(parfume);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParfumeExists(int id)
        {
            return _context.Parfumes.Any(e => e.ParfumeId == id);
        }
    }
}
