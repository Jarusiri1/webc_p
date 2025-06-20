using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class UserGroupsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserGroupsModel> _logger;

        public UserGroupsModel(AppDbContext context, ILogger<UserGroupsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<UserGroup> UserGroups { get; set; } = new List<UserGroup>(); // ✅ ใช้ได้
        public SelectList GroupSelectList { get; set; } = default!;

        [BindProperty]
        public UserGroup NewGroup { get; set; } = new();

        [BindProperty]
        public UserGroup EditGroup { get; set; } = new();

        [BindProperty]
        public Guid DeleteId { get; set; }

        public async Task OnGetAsync(Guid? groupId = null)
{
    var employeeNo = HttpContext.Session.GetString("EmployeeNo");
    if (string.IsNullOrEmpty(employeeNo))
    {
        Response.Redirect("/Login");
        return;
    }

    await LoadDataAsync(employeeNo, groupId);
}


private async Task LoadDataAsync(string employeeNo, Guid? groupId = null)
{
    GroupSelectList = new SelectList(await _context.Groups.ToListAsync(), "GroupId", "GroupName");

    var query = _context.UserGroups
        .Include(ug => ug.Group)
        .AsQueryable();

    if (groupId.HasValue)
    {
        query = query.Where(ug => ug.GroupId == groupId);
    }

    UserGroups = await query.OrderBy(ug => ug.Group!.GroupName).ToListAsync();
}


        private async Task LoadDataAsync()
        {
            GroupSelectList = new SelectList(await _context.Groups.ToListAsync(), "GroupId", "GroupName");
            UserGroups = await _context.UserGroups
                .Include(ug => ug.Group)
                .OrderBy(ug => ug.Group!.GroupName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            var employeeNo = HttpContext.Session.GetString("EmployeeNo");
            if (string.IsNullOrEmpty(employeeNo))
            {
                return RedirectToPage("/Login");
            }
            await LoadDataAsync();

            ModelState.Clear();
            await TryUpdateModelAsync(
                NewGroup,
                "NewGroup",
                m => m.EmployeeNo,
                m => m.GroupId,
                 m => m.FullName
            );

            if (string.IsNullOrWhiteSpace(NewGroup.EmployeeNo))
            {
                ModelState.AddModelError("NewGroup.EmployeeNo", "กรุณากรอกรหัสพนักงาน");
            }

            if (NewGroup.GroupId == Guid.Empty)
            {
                ModelState.AddModelError("NewGroup.GroupId", "กรุณาเลือกกลุ่ม");
            }

            bool duplicate = await _context.UserGroups.AnyAsync(
                ug => ug.EmployeeNo == NewGroup.EmployeeNo && ug.GroupId == NewGroup.GroupId);

            if (duplicate)
            {
                ModelState.AddModelError("NewGroup.EmployeeNo", "ผู้ใช้งานนี้อยู่ในกลุ่มนี้แล้ว");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Add validation failed: {errors}", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewData["ShowAddModal"] = true;
                return Page();
            }

            NewGroup.UserGroupId = Guid.NewGuid();
            _context.UserGroups.Add(NewGroup);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added UserGroup: {empNo} to GroupId: {groupId}", NewGroup.EmployeeNo, NewGroup.GroupId);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
{
    var employeeNo = HttpContext.Session.GetString("EmployeeNo");
    if (string.IsNullOrEmpty(employeeNo))
    {
        return RedirectToPage("/Login");
    }

    await LoadDataAsync(employeeNo);

    if (EditGroup == null || EditGroup.UserGroupId == Guid.Empty)
    {
        ViewData["ShowEditModal"] = true;
        return Page();
    }

    ModelState.Clear();
    await TryUpdateModelAsync(
        EditGroup,
        "EditGroup",
        m => m.EmployeeNo,
        m => m.GroupId,
        m => m.FullName
    );

    if (string.IsNullOrWhiteSpace(EditGroup.EmployeeNo))
    {
        ModelState.AddModelError("EditGroup.EmployeeNo", "กรุณากรอกรหัสพนักงาน");
    }

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Edit validation failed.");
        ViewData["ShowEditModal"] = true;
        return Page();
    }

    bool exists = await _context.UserGroups.AnyAsync(
        ug => ug.UserGroupId != EditGroup.UserGroupId &&
              ug.EmployeeNo == EditGroup.EmployeeNo &&
              ug.GroupId == EditGroup.GroupId);

    if (exists)
    {
        ModelState.AddModelError("EditGroup.EmployeeNo", "ข้อมูลซ้ำกับรายการที่มีอยู่แล้ว");
        ViewData["ShowEditModal"] = true;
        return Page();
    }

    var entity = await _context.UserGroups.FindAsync(EditGroup.UserGroupId);
    if (entity is null) return NotFound();

    entity.EmployeeNo = EditGroup.EmployeeNo.Trim();
    entity.GroupId = EditGroup.GroupId;
    entity.FullName = EditGroup.FullName;

    await _context.SaveChangesAsync();
    _logger.LogInformation("Edited UserGroup: {id}", entity.UserGroupId);

    return RedirectToPage();
}


        public async Task<IActionResult> OnPostDeleteAsync(Guid userGroupId)
{
    var employeeNo = HttpContext.Session.GetString("EmployeeNo");
    if (string.IsNullOrEmpty(employeeNo))
    {
        return RedirectToPage("/Login");
    }

    await LoadDataAsync(employeeNo);

    if (userGroupId == Guid.Empty)
    {
        _logger.LogWarning("DeleteId is empty");
        return Page();
    }

    var entity = await _context.UserGroups.FindAsync(userGroupId);
    if (entity != null)
    {
        _context.UserGroups.Remove(entity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted UserGroup: {id}", userGroupId);
    }

    return RedirectToPage();
}
    }
}
