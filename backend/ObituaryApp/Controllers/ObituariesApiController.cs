using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObituaryApp.Data;
using ObituaryApp.Models;

namespace ObituaryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObituariesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ObituariesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ObituariesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Obituary>>> GetObituaries()
        {
            return await _context.Obituaries.ToListAsync();
        }

        // GET: api/ObituariesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Obituary>> GetObituary(int id)
        {
            var obituary = await _context.Obituaries.FindAsync(id);

            if (obituary == null)
            {
                return NotFound();
            }

            return obituary;
        }

        // PUT: api/ObituariesApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutObituary(int id, Obituary obituary)
        {
            if (id != obituary.Id)
            {
                return BadRequest();
            }

            _context.Entry(obituary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObituaryExists(id))
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

        // POST: api/ObituariesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Obituary>> PostObituary(Obituary obituary)
        {
            _context.Obituaries.Add(obituary);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetObituary", new { id = obituary.Id }, obituary);
        }

        // DELETE: api/ObituariesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObituary(int id)
        {
            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary == null)
            {
                return NotFound();
            }

            _context.Obituaries.Remove(obituary);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ObituaryExists(int id)
        {
            return _context.Obituaries.Any(e => e.Id == id);
        }
    }
}
