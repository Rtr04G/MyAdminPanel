using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoctorPanel.Models;

namespace DoctorPanel.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly AppDbContext _context;

        public AdminUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AdminUsers
        public async Task<IActionResult> Index()
        {
              return _context.AdminUsers != null ? 
                          View(await _context.AdminUsers.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.AdminUsers'  is null.");
        }

        // GET: AdminUsers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.AdminUsers == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // GET: AdminUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SurName,MiddleName,Phone,Email")] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminUser);
        }

        // GET: AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AdminUsers == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return NotFound();
            }
            return View(adminUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,SurName,MiddleName,Phone,Email")] AdminUser adminUser)
        {
            if (id != adminUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminUserExists(adminUser.Id))
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
            return View(adminUser);
        }

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.AdminUsers == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdminUsers == null)
            {
                return Problem("Entity set 'AppDbContext.AdminUsers'  is null.");
            }
            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser != null)
            {
                _context.AdminUsers.Remove(adminUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminUserExists(string id)
        {
          return (_context.AdminUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
