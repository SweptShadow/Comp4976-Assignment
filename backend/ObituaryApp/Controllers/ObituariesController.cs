using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Obituaries.ToListAsync());
        }

        // GET: Obituaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obituary = await _context.Obituaries
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,DateOfBirth,DateOfDeath,Biography,PhotoPath,CreatedBy,CreatedDate,ModifiedDate")] Obituary obituary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obituary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obituary);
        }

        // GET: Obituaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary == null)
            {
                return NotFound();
            }
            return View(obituary);
        }

        // POST: Obituaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth,DateOfDeath,Biography,PhotoPath,CreatedBy,CreatedDate,ModifiedDate")] Obituary obituary)
        {
            if (id != obituary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obituary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObituaryExists(obituary.Id))
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
            return View(obituary);
        }

        // GET: Obituaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obituary = await _context.Obituaries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obituary == null)
            {
                return NotFound();
            }

            return View(obituary);
        }

        // POST: Obituaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obituary = await _context.Obituaries.FindAsync(id);
            if (obituary != null)
            {
                _context.Obituaries.Remove(obituary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObituaryExists(int id)
        {
            return _context.Obituaries.Any(e => e.Id == id);
        }
    }
}
