using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class AppModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppModel> _logger;

        public AppModel(AppDbContext context, ILogger<AppModel> logger)
        {
            _context = context;
            _logger = logger;
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
            _logger.LogInformation("OnPostCreateAsync start");
            Applications = _context.Applications.ToList();

            ModelState.Clear();
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

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create validation failed: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            _context.Applications.Add(NewApplication);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Create saved successfully: {Id}", NewApplication.Id);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            _logger.LogInformation("OnPostEditAsync start: EditApplication.Id={Id}", EditApplication?.Id);
            Applications = _context.Applications.ToList();

            if (EditApplication == null)
            {
                _logger.LogWarning("EditApplication is null");
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            try
            {
                var app = await _context.Applications.FindAsync(EditApplication.Id);
                if (app == null)
                {
                    _logger.LogError("Edit failed: Application not found Id={Id}", EditApplication.Id);
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Edit validation failed: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    ViewData["ShowEditModal"] = true;
                    return Page();
                }

                app.ApplicationId = EditApplication.ApplicationId;
                app.ApplicationName = EditApplication.ApplicationName;
                app.Status = EditApplication.Status;
                app.Description = EditApplication.Description;
                app.ContactName = EditApplication.ContactName;
                app.Telephone = EditApplication.Telephone;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Edit saved successfully Id={Id}", app.Id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostEditAsync for Id={Id}", EditApplication.Id);
                TempData["Error"] = "เกิดข้อผิดพลาดขณะบันทึกแก้ไข: " + ex.Message;
                ViewData["ShowEditModal"] = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            _logger.LogInformation("OnPostDeleteAsync start: DeleteId={Id}", DeleteId);
            if (DeleteId <= 0)
            {
                _logger.LogWarning("DeleteId invalid: {Id}", DeleteId);
                return BadRequest();
            }

            try
            {
                var app = await _context.Applications.FindAsync(DeleteId);
                if (app == null)
                {
                    _logger.LogError("Delete failed: Application not found Id={Id}", DeleteId);
                    return NotFound();
                }

                _context.Applications.Remove(app);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Delete succeeded Id={Id}", DeleteId);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostDeleteAsync for Id={Id}", DeleteId);
                TempData["Error"] = "เกิดข้อผิดพลาดขณะลบ: " + ex.Message;
                return RedirectToPage();
            }
        }
    }
}