using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Data;
using MyWebApp.Models;
using Microsoft.EntityFrameworkCore;

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
        public Permission EditPermission { get; set; } = new();

        [BindProperty]
        public Guid DeleteId { get; set; }

        public string? LoggedEmployeeNo { get; set; }

        public void OnGet()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
            {
                Response.Redirect("/Login");
                return;
            }

            LoggedEmployeeNo = employeeNo;

            // Reset ฟอร์มเพิ่มสิทธิ์
            NewPermission = new Permission
            {
                PermissionId = Guid.Empty,
                ApplicationId = Guid.Empty,
                PermissionName = string.Empty,
                Description = string.Empty
            };

            // แสดงเฉพาะสิทธิ์ที่ user คนนี้สร้างเท่านั้น
            Permissions = _context.Permissions
                .Where(p => p.CreateBy == employeeNo)
                .ToList();

            ViewData["ApplicationList"] = _context.Applications.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
                return RedirectToPage("/Login");

            // โหลดรายการสิทธิ์เฉพาะของ user นี้
            Permissions = _context.Permissions
                .Where(p => p.CreateBy == employeeNo)
                .ToList();

            ViewData["ApplicationList"] = _context.Applications.ToList();

            // Validate ข้อมูล
            ModelState.Clear();
            await TryUpdateModelAsync(
                NewPermission,
                "NewPermission",
                m => m.ApplicationId,
                m => m.PermissionName,
                m => m.Description
            );

            if (NewPermission.ApplicationId == Guid.Empty)
                ModelState.AddModelError("NewPermission.ApplicationId", "กรุณาเลือกแอปพลิเคชัน");

            if (string.IsNullOrWhiteSpace(NewPermission.PermissionName))
                ModelState.AddModelError("NewPermission.PermissionName", "กรุณากรอกชื่อสิทธิ์");

            if (!ModelState.IsValid)
            {
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            bool appExists = _context.Applications.Any(a => a.ApplicationId == NewPermission.ApplicationId);
            if (!appExists)
            {
                ModelState.AddModelError("NewPermission.ApplicationId", "ไม่พบ Application ID นี้ในระบบ");
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            NewPermission.PermissionId = Guid.NewGuid();
            NewPermission.CreateDate = DateTime.Now;
            NewPermission.CreateBy = employeeNo;
            NewPermission.UpdateDate = DateTime.Now;
            NewPermission.UpdateBy = employeeNo;

            _context.Permissions.Add(NewPermission);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
                return RedirectToPage("/Login");

            Permissions = _context.Permissions
                .Where(p => p.CreateBy == employeeNo)
                .ToList();

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

            // ดึงเฉพาะสิทธิ์ที่เจ้าตัวเป็นคนสร้าง
            var p = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.PermissionId == EditPermission.PermissionId &&
                p.CreateBy == employeeNo);

            if (p == null) return NotFound();

            p.PermissionName = EditPermission.PermissionName;
            p.Description = EditPermission.Description;
            p.UpdateDate = DateTime.Now;
            p.UpdateBy = employeeNo;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
                return RedirectToPage("/Login");

            if (DeleteId == Guid.Empty)
                return BadRequest();

            // ดึงเฉพาะสิทธิ์ของพนักงานนั้นเพื่อป้องกันลบข้ามคน
            var perm = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.PermissionId == DeleteId && p.CreateBy == employeeNo);

            if (perm != null)
            {
                _context.Permissions.Remove(perm);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

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
