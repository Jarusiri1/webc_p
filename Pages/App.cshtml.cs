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
            if (TempData["EmployeeNo"] == null)
            {
                Response.Redirect("/Login");
                return;
            }

            var employeeNo = TempData["EmployeeNo"]?.ToString();
            TempData.Keep("EmployeeNo");

            // ⭐ เพิ่มส่วนนี้ - สร้าง NewApplication ใหม่ที่มีค่าว่าง
            NewApplication = new Application
            {
                ApplicationId = Guid.Empty, // หรือใช้ default(Guid)
                ApplicationName = string.Empty,
                ApplicationStatus = string.Empty,
                Description = string.Empty,
                ContactName = string.Empty,
                Telephone = string.Empty
            };

            try
            {
                Applications = _context.Applications.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading applications for EmployeeNo={EmployeeNo}", employeeNo);
                throw;
            }
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
                m => m.ApplicationStatus,
                m => m.Description,
                m => m.ContactName,
                m => m.Telephone
            );

            //เพิ่มการตรวจสอบ ApplicationId ว่างหรือเป็น Guid.Empty
            if (NewApplication.ApplicationId == Guid.Empty)
            {
                ModelState.AddModelError("NewApplication.ApplicationId", "กรุณากรอกรหัสแอปพลิเคชัน");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create validation failed: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            //ตรวจสอบว่า ApplicationId ซ้ำหรือไม่
            var existingApp = await _context.Applications.FindAsync(NewApplication.ApplicationId);
            if (existingApp != null)
            {
                ModelState.AddModelError("NewApplication.ApplicationId", "ApplicationId นี้มีอยู่แล้ว");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            NewApplication.CreateBy = "System";
            NewApplication.CreatedDate = DateTime.Now;
            _context.Applications.Add(NewApplication);

            // เพิ่ม ApplicationAdmin
            /*
            var employeeNo = TempData["EmployeeNo"]?.ToString();
            TempData.Keep("EmployeeNo");

            if (!string.IsNullOrEmpty(employeeNo))
            {
                var admin = new ApplicationAdmin
                {
                    ApplicationAdminId = Guid.NewGuid(),
                    ApplicationId = NewApplication.ApplicationId,  // เก็บเป็น Guid โดยตรง,
                    EmployeeNo = employeeNo,
                    FullName = employeeNo, // เพิ่ม
                    CreateDate = DateTime.Now, // เพิ่ม
                    CreateBy = employeeNo, // เพิ่ม
                    UpdateDate = DateTime.Now, //  เพิ่ม
                    UpdateBy = employeeNo //เพิ่ม
                };
                _context.ApplicationAdmins.Add(admin);
            }
            */

            await _context.SaveChangesAsync();
            _logger.LogInformation("Create saved successfully: {Id}", NewApplication.ApplicationId);

            return RedirectToPage();
        }

        //เพิ่ม method สำหรับรีเซ็ต NewApplication
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
            _logger.LogInformation("OnPostEditAsync start: EditApplication.ApplicationId={Id}", EditApplication?.ApplicationId);
            Applications = _context.Applications.ToList();

            if (EditApplication == null || EditApplication.ApplicationId == Guid.Empty)
            {
                _logger.LogWarning("EditApplication is null");
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
                _logger.LogWarning("Edit validation failed: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            try
            {
                var app = await _context.Applications.FindAsync(EditApplication.ApplicationId);

                if (app == null)
                {
                    _logger.LogError("Edit failed: Application not found Id={Id}", EditApplication.ApplicationId);
                    return NotFound();
                }

                app.ApplicationId = EditApplication.ApplicationId;
                app.ApplicationName = EditApplication.ApplicationName;
                app.ApplicationStatus = EditApplication.ApplicationStatus;
                app.Description = EditApplication.Description;
                app.ContactName = EditApplication.ContactName;
                app.Telephone = EditApplication.Telephone;

                app.UpdateBy = "System";
                app.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Edit saved successfully Id={Id}", app.ApplicationId);
                
                // ⭐ รีเซ็ต NewApplication หลังจากแก้ไขสำเร็จ
                ResetNewApplication();
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostEditAsync for Id={Id}", EditApplication.ApplicationId);
                TempData["Error"] = "เกิดข้อผิดพลาดขณะบันทึกแก้ไข: " + ex.Message;
                ViewData["ShowEditModal"] = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            _logger.LogInformation("OnPostDeleteAsync start: DeleteId={Id}", DeleteId);
            if (DeleteId == Guid.Empty)
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
                
                // ⭐ รีเซ็ต NewApplication หลังจากลบสำเร็จ
                ResetNewApplication();
                
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