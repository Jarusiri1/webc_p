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
    public class SystemAdminsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SystemAdminsModel> _logger;

        public SystemAdminsModel(AppDbContext context, ILogger<SystemAdminsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<ApplicationAdmin> ApplicationAdmins { get; set; } = new();

        [BindProperty]
        public ApplicationAdmin NewApplicationAdmin { get; set; } = new();

        [BindProperty]
        public ApplicationAdmin? EditApplicationAdmin { get; set; }

        [BindProperty]
        public Guid DeleteId { get; set; }

        public void OnGet()
        {
            if (TempData["EmployeeNo"] == null)
            {
                Response.Redirect("/Login");
                return;
            }

            TempData.Keep("EmployeeNo");

            // รีเซ็ต NewApplicationAdmin
            NewApplicationAdmin = new ApplicationAdmin
            {
                ApplicationAdminId = Guid.Empty,
                ApplicationId = Guid.Empty, // เปลี่ยนเป็น Guid.Empty
                EmployeeNo = string.Empty,
                FullName = string.Empty
            };

            // ดึงรายการผู้ดูแลระบบทั้งหมด
            ApplicationAdmins = _context.ApplicationAdmins.ToList();
            
            // ดึงรายการแอปทั้งหมดเพื่อใช้ใน dropdown
            ViewData["ApplicationList"] = _context.Applications.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var employeeNo = TempData["EmployeeNo"]?.ToString();
            TempData.Keep("EmployeeNo");

            // โหลดข้อมูลใหม่สำหรับการแสดงผล
            ApplicationAdmins = _context.ApplicationAdmins.ToList();
            ViewData["ApplicationList"] = _context.Applications.ToList();

            // เคลียร์ ModelState และใช้ TryUpdateModelAsync
            ModelState.Clear();
            await TryUpdateModelAsync(
                NewApplicationAdmin,
                "NewApplicationAdmin",
                m => m.ApplicationId,
                m => m.EmployeeNo,
                m => m.FullName
            );

            // Debug log
            _logger.LogInformation("==== [DEBUG] POST: Create ApplicationAdmin ====");
            _logger.LogInformation("ApplicationId: {ApplicationId}", NewApplicationAdmin.ApplicationId);
            _logger.LogInformation("EmployeeNo: {EmployeeNo}", NewApplicationAdmin.EmployeeNo ?? "null");
            _logger.LogInformation("FullName: {FullName}", NewApplicationAdmin.FullName ?? "null");
            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);

            // ตรวจสอบข้อมูลที่จำเป็น
            if (NewApplicationAdmin.ApplicationId == Guid.Empty)
            {
                ModelState.AddModelError("NewApplicationAdmin.ApplicationId", "กรุณาเลือกแอปพลิเคชัน");
            }

            if (string.IsNullOrWhiteSpace(NewApplicationAdmin.EmployeeNo))
            {
                ModelState.AddModelError("NewApplicationAdmin.EmployeeNo", "กรุณากรอกรหัสพนักงาน");
            }

            // ทำให้ FullName เป็น optional
            // if (string.IsNullOrWhiteSpace(NewApplicationAdmin.FullName))
            // {
            //     ModelState.AddModelError("NewApplicationAdmin.FullName", "กรุณากรอกชื่อ-นามสกุล");
            // }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                    foreach (var error in state.Value.Errors)
                        _logger.LogWarning("[VALIDATION ERROR] {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);

                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // ตรวจสอบว่า Application ID มีอยู่จริงหรือไม่
            var appExists = _context.Applications.Any(a => a.ApplicationId == NewApplicationAdmin.ApplicationId);
            if (!appExists)
            {
                ModelState.AddModelError("NewApplicationAdmin.ApplicationId", "ไม่พบ Application ID นี้ในระบบ");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // ตรวจสอบว่าผู้ดูแลคนนี้มีอยู่ในแอปนี้แล้วหรือไม่
            var existingAdmin = _context.ApplicationAdmins.Any(a => 
                a.ApplicationId == NewApplicationAdmin.ApplicationId && 
                a.EmployeeNo == NewApplicationAdmin.EmployeeNo);
            if (existingAdmin)
            {
                ModelState.AddModelError("NewApplicationAdmin.EmployeeNo", "พนักงานคนนี้เป็นผู้ดูแลแอปนี้อยู่แล้ว");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // สร้างข้อมูลใหม่
            NewApplicationAdmin.ApplicationAdminId = Guid.NewGuid();
            NewApplicationAdmin.CreateDate = DateTime.Now;
            NewApplicationAdmin.CreateBy = employeeNo ?? "admin";
            NewApplicationAdmin.UpdateDate = DateTime.Now;
            NewApplicationAdmin.UpdateBy = employeeNo ?? "admin";

            _context.ApplicationAdmins.Add(NewApplicationAdmin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ApplicationAdmin created successfully!");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            ApplicationAdmins = _context.ApplicationAdmins.ToList();
            ViewData["ApplicationList"] = _context.Applications.ToList();

            if (EditApplicationAdmin == null || EditApplicationAdmin.ApplicationAdminId == Guid.Empty)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            if (!ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var admin = await _context.ApplicationAdmins.FindAsync(EditApplicationAdmin.ApplicationAdminId);
            if (admin == null) return NotFound();

            admin.EmployeeNo = EditApplicationAdmin.EmployeeNo;
            admin.FullName = EditApplicationAdmin.FullName;
            admin.UpdateDate = DateTime.Now;
            admin.UpdateBy = TempData["EmployeeNo"]?.ToString() ?? "admin";

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (DeleteId == Guid.Empty)
                return BadRequest();

            var admin = await _context.ApplicationAdmins.FindAsync(DeleteId);
            if (admin != null)
            {
                _context.ApplicationAdmins.Remove(admin);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // เพิ่ม method สำหรับรีเซ็ต NewApplicationAdmin
        private void ResetNewApplicationAdmin()
        {
            NewApplicationAdmin = new ApplicationAdmin
            {
                ApplicationAdminId = Guid.Empty,
                ApplicationId = Guid.Empty, // ⭐ เปลี่ยนเป็น Guid.Empty
                EmployeeNo = string.Empty,
                FullName = string.Empty
            };
        }
    }
}