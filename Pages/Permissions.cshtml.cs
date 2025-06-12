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
        public Permission? EditPermission { get; set; }

        [BindProperty]
        public Guid DeleteId { get; set; }

        public string? LoggedEmployeeNo { get; set; }

        public void OnGet()
        {
            if (TempData["EmployeeNo"] == null)
            {
                Response.Redirect("/Login");
                return;
            }

            TempData.Keep("EmployeeNo");

            //เพิ่มการรีเซ็ต NewPermission
            NewPermission = new Permission
            {
                PermissionId = Guid.Empty,
                ApplicationId = Guid.Empty,
                PermissionName = string.Empty,
                Description = string.Empty
            };

            // ดึงรายการสิทธิทั้งหมด
            Permissions = _context.Permissions.ToList();
            
            // ดึงรายการแอปทั้งหมดเพื่อใช้ใน dropdown
            ViewData["ApplicationList"] = _context.Applications.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var employeeNo = TempData["EmployeeNo"]?.ToString();
            TempData.Keep("EmployeeNo");

            // โหลดข้อมูลใหม่สำหรับการแสดงผล
            Permissions = _context.Permissions.ToList();
            ViewData["ApplicationList"] = _context.Applications.ToList();

            //เคลียร์ ModelState และใช้ TryUpdateModelAsync
            ModelState.Clear();
            await TryUpdateModelAsync(
                NewPermission,
                "NewPermission",
                m => m.ApplicationId,
                m => m.PermissionName,
                m => m.Description
            );

            // Debug log
            Console.WriteLine("==== [DEBUG] POST: Create Permission ====");
            Console.WriteLine("ApplicationId: " + NewPermission.ApplicationId);
            Console.WriteLine("PermissionName: " + (NewPermission.PermissionName ?? "null"));
            Console.WriteLine("Description: " + (NewPermission.Description ?? "null"));
            Console.WriteLine("ModelState.IsValid: " + ModelState.IsValid);

            Console.WriteLine("==== [DEBUG] FORM CONTENT ====");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"[FORM] {key} = {Request.Form[key]}");
            }

            //เพิ่มการตรวจสอบเฉพาะสิ่งที่จำเป็น
            if (NewPermission.ApplicationId == Guid.Empty)
            {
                ModelState.AddModelError("NewPermission.ApplicationId", "กรุณาเลือกแอปพลิเคชัน");
            }

            if (string.IsNullOrWhiteSpace(NewPermission.PermissionName))
            {
                ModelState.AddModelError("NewPermission.PermissionName", "กรุณากรอกชื่อสิทธิ์");
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                    foreach (var error in state.Value.Errors)
                        Console.WriteLine($"[VALIDATION ERROR] {state.Key}: {error.ErrorMessage}");

                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // ตรวจสอบว่า Application ID มีอยู่จริงหรือไม่
            bool appExists = _context.Applications.Any(a => a.ApplicationId == NewPermission.ApplicationId);
            if (!appExists)
            {
                ModelState.AddModelError("NewPermission.ApplicationId", "ไม่พบ Application ID นี้ในระบบ");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            // สร้างข้อมูลใหม่
            NewPermission.PermissionId = Guid.NewGuid();
            NewPermission.CreateDate = DateTime.Now;
            NewPermission.CreateBy = employeeNo ?? "admin";
            
            // ⭐ เพิ่มค่า UpdateDate และ UpdateBy
            NewPermission.UpdateDate = DateTime.Now;
            NewPermission.UpdateBy = employeeNo ?? "admin";

            _context.Permissions.Add(NewPermission);
            await _context.SaveChangesAsync();

            Console.WriteLine("Permission created successfully!");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            Permissions = _context.Permissions.ToList();
            ViewData["ApplicationList"] = _context.Applications.ToList();

            if (EditPermission == null || EditPermission.PermissionId == Guid.Empty)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            if (!ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                return Page();
            }

            var p = await _context.Permissions.FindAsync(EditPermission.PermissionId);
            if (p == null) return NotFound();

            p.PermissionName = EditPermission.PermissionName;
            p.Description = EditPermission.Description;

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

        //เพิ่ม method สำหรับรีเซ็ต NewPermission
        private void ResetNewPermission()
        {
            NewPermission = new Permission
            {
                PermissionId = Guid.Empty,
                ApplicationId = Guid.Empty,
                PermissionName = string.Empty,
                Description = string.Empty
            };
        }
    }
}