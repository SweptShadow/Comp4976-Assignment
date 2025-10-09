using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ObituaryApp.Extensions;
using ObituaryApp.Data;
using ObituaryApp.Models;

namespace ObituaryApp.Controllers
{
    public class ObituariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObituariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Obituaries
        /**
         * Lists obituaries with optional search and pagination.
         * Anonymous users can view.
         */
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Obituaries.Include(o => o.CreatedByUser).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.FullName.Contains(search));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items);
        }


        // GET: Obituaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obituary = await _context.Obituaries
                .Include(o => o.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obituary == null)
            {
                return NotFound();
            }

            return View(obituary);
        }

        // GET: Obituaries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Obituaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /**
         * Creates a new obituary.
         * Authenticated users: Sets CreatedBy automatically from logged-in user.
         * Non-authenticated users: Must provide SubmittedByName.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,DateOfBirth,DateOfDeath,Biography,SubmittedByName")] Obituary obituary, IFormFile? photoFile)
        {
            if (obituary.DateOfDeath < obituary.DateOfBirth)
            {
                ModelState.AddModelError(nameof(obituary.DateOfDeath), "Date of death cannot be before date of birth.");
            }

            // If user is not authenticated, require SubmittedByName
            if (User.Identity?.IsAuthenticated != true && string.IsNullOrWhiteSpace(obituary.SubmittedByName))
            {
                ModelState.AddModelError(nameof(obituary.SubmittedByName), "Submitted By name is required for anonymous submissions.");
            }

            if (!ModelState.IsValid) return View(obituary);

            // Set CreatedBy for authenticated users
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.GetUserId();
                if (userId == null) return Forbid();
                obituary.CreatedBy = userId;
            }

            obituary.CreatedDate = DateTime.UtcNow;
            obituary.ModifiedDate = DateTime.UtcNow;
            // Handle uploaded photo file (if provided)
            if (photoFile != null && photoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }
                obituary.PhotoPath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
            }

            _context.Add(obituary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Obituaries/Edit/5
        /**
         * Returns edit form.
         * Only creator or admin can access.
         */
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary == null) return NotFound();

            if (!CanModify(obituary)) return Forbid();

            return View(obituary);
        }


        // POST: Obituaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /**
         * Updates an obituary.
         * Only creator or admin can modify.
         * Updates ModifiedDate automatically.
         */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth,DateOfDeath,Biography")] Obituary obituary, IFormFile? photoFile)
        {
            if (id != obituary.Id) return NotFound();

            var existing = await _context.Obituaries.FindAsync(id);
            if (existing == null) return NotFound();

            if (!CanModify(existing)) return Forbid();

            if (obituary.DateOfDeath < obituary.DateOfBirth)
            {
                ModelState.AddModelError(nameof(obituary.DateOfDeath), "Date of death cannot be before date of birth.");
            }

            if (!ModelState.IsValid) return View(obituary);

            existing.FullName = obituary.FullName;
            existing.DateOfBirth = obituary.DateOfBirth;
            existing.DateOfDeath = obituary.DateOfDeath;
            existing.Biography = obituary.Biography;
            existing.ModifiedDate = DateTime.UtcNow;

            // Handle uploaded photo (replace existing if provided)
            if (photoFile != null && photoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // Delete old file if exists
                if (!string.IsNullOrWhiteSpace(existing.PhotoPath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.PhotoPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldPath))
                    {
                        try { System.IO.File.Delete(oldPath); } catch { /* ignore errors */ }
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }
                existing.PhotoPath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Obituaries/Delete/5
        /**
         * Returns delete confirmation.
         * Only creator or admin can access.
         */
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var obituary = await _context.Obituaries.FirstOrDefaultAsync(m => m.Id == id);
            if (obituary == null) return NotFound();

            if (!CanModify(obituary)) return Forbid();

            return View(obituary);
        }

        // POST: Obituaries/Delete/5
        /**
         * Deletes an obituary.
         * Only creator or admin can perform.
         */
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary == null) return NotFound();

            if (!CanModify(obituary)) return Forbid();

            _context.Obituaries.Remove(obituary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /**
         * Returns true if current user is admin or the creator of the obituary.
         */
        private bool CanModify(Obituary obituary)
        {
            var userId = User.GetUserId();
            return User.IsInRole("admin") || (userId != null && obituary.CreatedBy == userId);
        }
    }
}
