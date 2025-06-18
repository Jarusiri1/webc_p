using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages
{
    public class UserGroupsModel : PageModel
    {
        private readonly AppDbContext _context;
        
        public UserGroupsModel(AppDbContext context) => _context = context;

        [BindProperty]
        public UserGroup NewGroup { get; set; } = new();

        public IList<UserGroup> UserGroups { get; set; } = default!;
        
        public SelectList GroupSelectList { get; set; } = default!;

        public void OnGet() => LoadData();

        public IActionResult OnPostAdd()
{
    if (!ModelState.IsValid)
    {
        LoadData();
        return Page();
    }

    var employeeNo = NewGroup.EmployeeNo.Trim();
    var groupId = NewGroup.GroupId;

    // ตรวจสอบซ้ำ ไม่ให้เพิ่มซ้ำ
    var exists = _context.UserGroups.Any(ug => ug.EmployeeNo == employeeNo && ug.GroupId == groupId);
    if (exists)
    {
        ModelState.AddModelError("NewGroup.EmployeeNo", "ผู้ใช้งานนี้อยู่ในกลุ่มนี้แล้ว");
        LoadData();
        ViewData["ShowAddModal"] = true;
        return Page();
    }

    // ✅ เพิ่มให้ครบทุก field ที่ต้องมี
    var userGroup = new UserGroup
    {
        UserGroupId = Guid.NewGuid(),
        EmployeeNo = employeeNo,
        GroupId = groupId
    };

    _context.UserGroups.Add(userGroup);
    _context.SaveChanges();

    return RedirectToPage();
}


        public IActionResult OnPostDelete(Guid id)
        {
            var entity = _context.UserGroups.Find(id);
            if (entity != null) 
            { 
                _context.UserGroups.Remove(entity); 
                _context.SaveChanges(); 
            }
            return RedirectToPage();
        }

        private void LoadData()
        {
            GroupSelectList = new SelectList(_context.Groups, "GroupId", "GroupName");
            UserGroups = _context.UserGroups.Include(ug => ug.Group).ToList();
        }
    }
}