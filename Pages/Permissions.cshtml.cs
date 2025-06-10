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

    Permissions = _context.Permissions.ToList();
    ViewData["ApplicationList"] = _context.Applications.ToList(); // ✅ Set ตรงนี้เพื่อใช้ใน View

    // 🔽 Debug log
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
    if (!ModelState.IsValid)
    {
        foreach (var state in ModelState)
            foreach (var error in state.Value.Errors)
                Console.WriteLine($"[VALIDATION ERROR] {state.Key}: {error.ErrorMessage}");

        ViewData["ShowAddModal"] = true;
        return Page();
    }

    bool appExists = _context.Applications.Any(a => a.ApplicationId == NewPermission.ApplicationId);
    if (!appExists)
    {
        ModelState.AddModelError("NewPermission.ApplicationId", "ไม่พบ Application ID นี้ในระบบ");
        ViewData["ShowAddModal"] = true;
        ViewData["ApplicationList"] = _context.Applications.ToList();
        return Page();
    }

    NewPermission.PermissionId = Guid.NewGuid();
    NewPermission.CreateDate = DateTime.Now;
    NewPermission.CreateBy = employeeNo ?? "admin";

    _context.Permissions.Add(NewPermission);
    await _context.SaveChangesAsync();

    return RedirectToPage();
}


        public async Task<IActionResult> OnPostEditAsync()
        {
            Permissions = _context.Permissions.ToList();

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
    }
}
