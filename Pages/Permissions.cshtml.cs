using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class PermissionsModel : PageModel
    {
        private readonly AppDbContext _context;
        public PermissionsModel(AppDbContext context) => _context = context;

        public List<Permission> Permissions { get; set; } = new();

        [BindProperty]
        public Permission NewPermission { get; set; } = new();

        [BindProperty]
        public Guid DeleteId { get; set; }

        public void OnGet()
        {
            Permissions = _context.Permissions.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Permissions = _context.Permissions.ToList();
            if (!ModelState.IsValid)
            {
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            _context.Permissions.Add(NewPermission);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (DeleteId == Guid.Empty)
                return BadRequest();

            var perm = await _context.Permissions.FindAsync(DeleteId);
            if (perm != null)
            {
                _context.Permissions.Remove(perm);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
