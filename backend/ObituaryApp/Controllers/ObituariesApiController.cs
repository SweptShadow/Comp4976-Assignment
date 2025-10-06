using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ObituaryApp.Extensions;
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
        /**
         * Returns a paged list of obituaries with optional name search.
         * Anonymous access allowed.
         */
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetObituaries([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Obituaries.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.FullName.Contains(search));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { page, pageSize, total, items });
        }


        // GET: api/ObituariesApi/5
        /**
         * Returns a single obituary by id.
         * Anonymous access allowed.
         */
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetObituary(int id)
        {
            var obituary = await _context.Obituaries.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            if (obituary == null) return NotFound();

            return Ok(obituary);
        }


        // PUT: api/ObituariesApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /**
         * Updates an obituary.
         * Only creator or admin can modify. JWT required.
         */
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutObituary(int id, [FromBody] Obituary input)
        {
            var existing = await _context.Obituaries.FindAsync(id);
            if (existing == null) return NotFound();

            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("admin");
            if (!isAdmin && (userId == null || existing.CreatedBy != userId)) return Forbid();

            if (input.DateOfDeath < input.DateOfBirth)
            {
                ModelState.AddModelError(nameof(input.DateOfDeath), "Date of death cannot be before date of birth.");
            }
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            existing.FullName = input.FullName;
            existing.DateOfBirth = input.DateOfBirth;
            existing.DateOfDeath = input.DateOfDeath;
            existing.Biography = input.Biography;
            existing.PhotoPath = input.PhotoPath;
            existing.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }


        // POST: api/ObituariesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /**
         * Creates a new obituary.
         * JWT required. Sets CreatedBy automatically.
         */
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostObituary([FromBody] Obituary obituary)
        {
            if (obituary.DateOfDeath < obituary.DateOfBirth)
            {
                ModelState.AddModelError(nameof(obituary.DateOfDeath), "Date of death cannot be before date of birth.");
            }
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return Forbid();

            obituary.Id = 0; // ensure new
            obituary.CreatedBy = userId;
            obituary.CreatedDate = DateTime.UtcNow;
            obituary.ModifiedDate = DateTime.UtcNow;

            _context.Obituaries.Add(obituary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetObituary), new { id = obituary.Id }, obituary);
        }


        // DELETE: api/ObituariesApi/5
        /**
         * Deletes an obituary.
         * Only creator or admin can delete. JWT required.
         */
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteObituary(int id)
        {
            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary == null) return NotFound();

            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("admin");
            if (!isAdmin && (userId == null || obituary.CreatedBy != userId)) return Forbid();

            _context.Obituaries.Remove(obituary);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

    }
}
