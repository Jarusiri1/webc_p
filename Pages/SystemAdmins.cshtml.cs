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
    // ✅ ตรวจสอบ Session ก่อน (เปลี่ยนจาก TempData เป็น Session)
    var employeeNo = HttpContext.Session.GetString("EmployeeNo");
    if (string.IsNullOrEmpty(employeeNo))
    {
        Response.Redirect("/Login");
        return;
    }

    // ✅ เก็บไว้ใช้งาน
    NewApplicationAdmin = new ApplicationAdmin
    {
        ApplicationAdminId = Guid.Empty,
        ApplicationId = Guid.Empty,
        EmployeeNo = string.Empty,
        FullName = string.Empty
    };

    ApplicationAdmins = _context.ApplicationAdmins.ToList();

    var allApplications = _context.Applications
        .OrderBy(a => a.ApplicationStatus == "ไม่ได้ใช้งาน")
        .ThenBy(a => a.ApplicationName)
        .ToList();

    ViewData["ApplicationList"] = allApplications;

    ViewData["ActiveApplicationList"] = allApplications
        .Where(a => a.ApplicationStatus == "ใช้งาน")
        .ToList();

    ViewData["TotalAdmins"] = ApplicationAdmins.Count;
    ViewData["TotalApps"] = allApplications.Count(a => a.ApplicationStatus == "ใช้งาน");
}


        public async Task<IActionResult> OnPostCreateAsync()
        {
            var employeeNo = TempData["EmployeeNo"]?.ToString();
            TempData.Keep("EmployeeNo");

            // โหลดข้อมูลใหม่สำหรับการแสดงผล
            ApplicationAdmins = _context.ApplicationAdmins.ToList();
            
            var allApplications = _context.Applications
                .OrderBy(a => a.ApplicationStatus == "ไม่ได้ใช้งาน")
                .ThenBy(a => a.ApplicationName)
                .ToList();
                
            ViewData["ApplicationList"] = allApplications;
            ViewData["ActiveApplicationList"] = allApplications
                .Where(a => a.ApplicationStatus == "ใช้งาน")
                .ToList();

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
            else
            {
                // 🔥 เพิ่ม: ตรวจสอบรูปแบบรหัสพนักงาน
                NewApplicationAdmin.EmployeeNo = NewApplicationAdmin.EmployeeNo.Trim();
                if (NewApplicationAdmin.EmployeeNo.Length < 3)
                {
                    ModelState.AddModelError("NewApplicationAdmin.EmployeeNo", "รหัสพนักงานต้องมีอย่างน้อย 3 ตัวอักษร");
                }
            }

            // 🔥 ปรับปรุง: ทำให้ FullName เป็น optional และ trim
            if (!string.IsNullOrWhiteSpace(NewApplicationAdmin.FullName))
            {
                NewApplicationAdmin.FullName = NewApplicationAdmin.FullName.Trim();
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                    foreach (var error in state.Value.Errors)
                        _logger.LogWarning("[VALIDATION ERROR] {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);

                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // ตรวจสอบว่า Application ID มีอยู่จริงและใช้งานอยู่หรือไม่
            var app = _context.Applications.FirstOrDefault(a => a.ApplicationId == NewApplicationAdmin.ApplicationId);
            if (app == null)
            {
                ModelState.AddModelError("NewApplicationAdmin.ApplicationId", "ไม่พบแอปพลิเคชันนี้ในระบบ");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            if (app.ApplicationStatus != "ใช้งาน")
            {
                ModelState.AddModelError("NewApplicationAdmin.ApplicationId", "ไม่สามารถเพิ่มผู้ดูแลสำหรับแอปพลิเคชันที่ไม่ได้ใช้งาน");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // ตรวจสอบว่าผู้ดูแลคนนี้มีอยู่ในแอปนี้แล้วหรือไม่
            var existingAdmin = _context.ApplicationAdmins.Any(a => 
                a.ApplicationId == NewApplicationAdmin.ApplicationId && 
                a.EmployeeNo.ToLower() == NewApplicationAdmin.EmployeeNo.ToLower());
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
            
            // 🔥 แก้ไข: เพิ่ม success message
            TempData["LoginMessage"] = $"เพิ่มผู้ดูแลระบบ {NewApplicationAdmin.EmployeeNo} สำเร็จ";
            
            return RedirectToPage("/SystemAdmins"); // 🔥 แก้ไข: ระบุหน้าอย่างชัดเจน
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            ApplicationAdmins = _context.ApplicationAdmins.ToList();
            
            var allApplications = _context.Applications
                .OrderBy(a => a.ApplicationStatus == "ไม่ได้ใช้งาน")
                .ThenBy(a => a.ApplicationName)
                .ToList();
                
            ViewData["ApplicationList"] = allApplications;
            ViewData["ActiveApplicationList"] = allApplications
                .Where(a => a.ApplicationStatus == "ใช้งาน")
                .ToList();

            if (EditApplicationAdmin == null || EditApplicationAdmin.ApplicationAdminId == Guid.Empty)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            // 🔥 เพิ่ม: ตรวจสอบข้อมูล
            if (!string.IsNullOrWhiteSpace(EditApplicationAdmin.EmployeeNo))
            {
                EditApplicationAdmin.EmployeeNo = EditApplicationAdmin.EmployeeNo.Trim();
            }
            if (!string.IsNullOrWhiteSpace(EditApplicationAdmin.FullName))
            {
                EditApplicationAdmin.FullName = EditApplicationAdmin.FullName.Trim();
            }

            if (!ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var admin = await _context.ApplicationAdmins.FindAsync(EditApplicationAdmin.ApplicationAdminId);
            if (admin == null) return NotFound();

            // 🔥 เพิ่ม: ตรวจสอบการซ้ำของรหัสพนักงาน (ยกเว้นตัวเอง)
            var duplicateEmployee = _context.ApplicationAdmins.Any(a => 
                a.ApplicationId == admin.ApplicationId && 
                a.EmployeeNo.ToLower() == EditApplicationAdmin.EmployeeNo.ToLower() &&
                a.ApplicationAdminId != EditApplicationAdmin.ApplicationAdminId);
                
            if (duplicateEmployee)
            {
                ModelState.AddModelError("EditApplicationAdmin.EmployeeNo", "รหัสพนักงานนี้มีอยู่ในแอปพลิเคชันนี้แล้ว");
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            admin.EmployeeNo = EditApplicationAdmin.EmployeeNo;
            admin.FullName = EditApplicationAdmin.FullName;
            admin.UpdateDate = DateTime.Now;
            admin.UpdateBy = TempData["EmployeeNo"]?.ToString() ?? "admin";

            await _context.SaveChangesAsync();
            
            TempData["LoginMessage"] = $"แก้ไขข้อมูลผู้ดูแลระบบ {admin.EmployeeNo} สำเร็จ";
            
            return RedirectToPage("/SystemAdmins"); // 🔥 แก้ไข: ระบุหน้าอย่างชัดเจน
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (DeleteId == Guid.Empty)
                return BadRequest();

            var admin = await _context.ApplicationAdmins.FindAsync(DeleteId);
            if (admin != null)
            {
                var employeeNo = admin.EmployeeNo;
                _context.ApplicationAdmins.Remove(admin);
                await _context.SaveChangesAsync();
                
                TempData["LoginMessage"] = $"ลบผู้ดูแลระบบ {employeeNo} สำเร็จ";
            }
            
            return RedirectToPage("/SystemAdmins"); // 🔥 แก้ไข: ระบุหน้าอย่างชัดเจน
        }

        // เพิ่ม method สำหรับรีเซ็ต NewApplicationAdmin
        private void ResetNewApplicationAdmin()
        {
            NewApplicationAdmin = new ApplicationAdmin
            {
                ApplicationAdminId = Guid.Empty,
                ApplicationId = Guid.Empty,
                EmployeeNo = string.Empty,
                FullName = string.Empty
            };
        }
    }
}