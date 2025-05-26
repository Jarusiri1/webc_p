using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Data;

namespace MyWebApp.Pages
{
    public class AppModel : PageModel
    {
        private readonly AppDbContext _context;

        public AppModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Application> Applications { get; set; } = new();

        [BindProperty]
        public Application NewApplication { get; set; } = new();

        [BindProperty]
        public Application? EditApplication { get; set; }

        [BindProperty]
        public int DeleteId { get; set; }

        public void OnGet()
        {
            Applications = _context.Applications.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            // Debug: log all form keys and values
            Console.WriteLine("===== Request.Form Keys =====");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"Key: {key} => {Request.Form[key]}");
            }
            Console.WriteLine("===== End Form Keys =====");

            // Clear existing ModelState to remove stale errors
            ModelState.Clear();

            // Bind the form values into NewApplication
            await TryUpdateModelAsync(
                NewApplication,
                "NewApplication",
                m => m.ApplicationId,
                m => m.ApplicationName,
                m => m.Status,
                m => m.Description,
                m => m.ContactName,
                m => m.Telephone
            );

            Console.WriteLine(">>> เริ่ม OnPostCreateAsync <<<");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState invalid");
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Error in {kvp.Key}: {err.ErrorMessage}");
                    }
                }
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            try
            {
                _context.Applications.Add(NewApplication);
                await _context.SaveChangesAsync();
                Console.WriteLine("บันทึกข้อมูลสำเร็จ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            Applications = _context.Applications.ToList();

            if (EditApplication == null || !ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var app = await _context.Applications.FindAsync(EditApplication.Id);
            if (app == null)
                return NotFound();

            app.ApplicationId = EditApplication.ApplicationId;
            app.ApplicationName = EditApplication.ApplicationName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "เกิดข้อผิดพลาดขณะบันทึก: " + ex.Message;
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var app = await _context.Applications.FindAsync(DeleteId);
            if (app == null)
                return NotFound();

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
