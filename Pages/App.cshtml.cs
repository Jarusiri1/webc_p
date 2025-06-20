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
        public Guid DeleteId { get; set; }

        public void OnGet()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
            {
                Response.Redirect("/Login");
                return;
            }

            Applications = _context.Applications
                .Where(a => a.CreateBy == employeeNo)
                .ToList();

            NewApplication = new Application
            {
                ApplicationId = Guid.Empty,
                ApplicationName = string.Empty,
                ApplicationStatus = string.Empty,
                Description = string.Empty,
                ContactName = string.Empty,
                Telephone = string.Empty
            };
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");

            if (string.IsNullOrEmpty(employeeNo))
            {
                return RedirectToPage("/Login");
            }

            ModelState.Clear();
            await TryUpdateModelAsync(
                NewApplication,
                "NewApplication",
                m => m.ApplicationId,
                m => m.ApplicationName,
                m => m.ApplicationStatus,
                m => m.Description,
                m => m.ContactName,
                m => m.Telephone
            );

            if (NewApplication.ApplicationId == Guid.Empty)
            {
                ModelState.AddModelError("NewApplication.ApplicationId", "กรุณากรอกรหัสแอปพลิเคชัน");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            var existingApp = await _context.Applications.FindAsync(NewApplication.ApplicationId);
            if (existingApp != null)
            {
                ModelState.AddModelError("NewApplication.ApplicationId", "ApplicationId นี้มีอยู่แล้ว");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            NewApplication.CreateBy = employeeNo;
            NewApplication.CreatedDate = DateTime.Now;
            _context.Applications.Add(NewApplication);

            var admin = new ApplicationAdmin
            {
                ApplicationAdminId = Guid.NewGuid(),
                ApplicationId = NewApplication.ApplicationId,
                EmployeeNo = employeeNo,
                FullName = HttpContext.Session.GetString("FullName") ?? "",
                CreateDate = DateTime.Now,
                CreateBy = employeeNo,
                UpdateDate = DateTime.Now,
                UpdateBy = employeeNo
            };
            //_context.ApplicationAdmins.Add(admin);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        private void ResetNewApplication()
        {
            NewApplication = new Application
            {
                ApplicationId = Guid.Empty,
                ApplicationName = string.Empty,
                ApplicationStatus = string.Empty,
                Description = string.Empty,
                ContactName = string.Empty,
                Telephone = string.Empty
            };
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            Applications = _context.Applications.ToList();

            if (EditApplication == null || EditApplication.ApplicationId == Guid.Empty)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            ModelState.Clear();
            await TryUpdateModelAsync(
                EditApplication,
                "EditApplication",
                m => m.ApplicationId,
                m => m.ApplicationName,
                m => m.ApplicationStatus,
                m => m.Description,
                m => m.ContactName,
                m => m.Telephone
            );

            if (!ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var app = await _context.Applications.FindAsync(EditApplication.ApplicationId);
            if (app == null)
            {
                return NotFound();
            }

            app.ApplicationName = EditApplication.ApplicationName;
            app.ApplicationStatus = EditApplication.ApplicationStatus;
            app.Description = EditApplication.Description;
            app.ContactName = EditApplication.ContactName;
            app.Telephone = EditApplication.Telephone;

            app.UpdateBy = HttpContext.Session.GetString("EmployeeNo") ?? "Unknown";
            app.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            ResetNewApplication();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (DeleteId == Guid.Empty)
            {
                return BadRequest();
            }

            var app = await _context.Applications.FindAsync(DeleteId);
            if (app == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();
            ResetNewApplication();
            return RedirectToPage();
        }
    }
}
