using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages
{
    public class GroupModel : PageModel
    {
        private readonly AppDbContext _context;

        public GroupModel(AppDbContext context)
        {
            _context = context;
        }

        // Properties สำหรับแสดงข้อมูล
        public List<Group> Groups { get; set; } = new();
        public List<Application> Applications { get; set; } = new();
        public Guid? SelectedApplicationId { get; set; }

        // Properties สำหรับ Form
        [BindProperty]
        public GroupCreateModel NewGroup { get; set; } = new();

        [BindProperty]
        public GroupEditModel EditGroup { get; set; } = new();

        [BindProperty]
        public Guid DeleteId { get; set; }

        public async Task OnGetAsync(Guid? applicationId = null)
        {
            SelectedApplicationId = applicationId;
            
            // โหลดแอปพลิเคชันทั้งหมด
            Applications = await _context.Applications
                .OrderBy(a => a.ApplicationName)
                .ToListAsync();

            // โหลดกลุ่มตามเงื่อนไข
            var query = _context.Groups.Include(g => g.Application).AsQueryable();
            
            if (applicationId.HasValue)
            {
                query = query.Where(g => g.ApplicationId == applicationId.Value);
            }

            Groups = await query
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

public async Task<IActionResult> OnPostCreateAsync()
{
    Console.WriteLine("✅ เรียก OnPostCreateAsync แล้ว");

    await LoadDataAsync(); // โหลด Applications และ Groups ใหม่

    ModelState.Clear();
    var updated = await TryUpdateModelAsync(
        NewGroup,
        "NewGroup",
        g => g.ApplicationId,
        g => g.GroupName,
        g => g.Description
    );

    if (!updated)
    {
        Console.WriteLine("❌ Binding model ไม่สำเร็จ");
    }

    if (!ModelState.IsValid)
    {
        Console.WriteLine("❌ ModelState ไม่ valid");
        foreach (var kv in ModelState)
        {
            foreach (var err in kv.Value.Errors)
            {
                Console.WriteLine($"- {kv.Key}: {err.ErrorMessage}");
            }
        }
        ViewData["ShowAddModal"] = true;
        return Page();
    }

    // ตรวจซ้ำ
    var exists = await _context.Groups.AnyAsync(g => g.GroupName == NewGroup.GroupName && g.ApplicationId == NewGroup.ApplicationId);
    if (exists)
    {
        ModelState.AddModelError("NewGroup.GroupName", "ชื่อกลุ่มนี้มีอยู่แล้วในแอปพลิเคชันนี้");
        ViewData["ShowAddModal"] = true;
        return Page();
    }
var employeeNo = TempData["EmployeeNo"]?.ToString();
TempData.Keep("EmployeeNo");

var group = new Group
{
    GroupId = Guid.NewGuid(),
    ApplicationId = NewGroup.ApplicationId,
    GroupName = NewGroup.GroupName,
    Description = NewGroup.Description,
    CreateBy = employeeNo ?? "System",
    CreateDate = DateTime.Now,
    UpdateBy = employeeNo ?? "System",
    UpdateDate = DateTime.Now
};

_context.Groups.Add(group);
await _context.SaveChangesAsync();
Console.WriteLine("✅ บันทึกกลุ่มใหม่เรียบร้อย");

return RedirectToPage(new { applicationId = group.ApplicationId });
}


        public async Task<IActionResult> OnPostEditAsync()
        {
            Console.WriteLine("✅ OnPostEditAsync เรียกใช้แล้ว");
            if (!ModelState.IsValid)
            {
                ViewData["ShowEditModal"] = true;
                await LoadDataAsync();
                return Page();
            }

            try
            {
                var group = await _context.Groups.FindAsync(EditGroup.GroupId);
                if (group == null)
                {
                    TempData["Error"] = "ไม่พบกลุ่มที่ต้องการแก้ไข";
                    return RedirectToPage(new { applicationId = EditGroup.ApplicationId });

                }

                // ตรวจสอบชื่อซ้ำ (ยกเว้นตัวเอง)
                var existingGroup = await _context.Groups
                    .FirstOrDefaultAsync(g => g.GroupName == EditGroup.GroupName && 
                                            g.ApplicationId == EditGroup.ApplicationId &&
                                            g.GroupId != EditGroup.GroupId);

                if (existingGroup != null)
                {
                    ModelState.AddModelError("EditGroup.GroupName", "ชื่อกลุ่มนี้มีอยู่แล้วในแอปพลิเคชันนี้");
                    ViewData["ShowEditModal"] = true;
                    await LoadDataAsync();
                    return Page();
                }

                group.ApplicationId = EditGroup.ApplicationId;
                group.GroupName = EditGroup.GroupName;
                group.Description = EditGroup.Description;
                group.UpdateBy = "System"; // ปรับให้เป็น User ที่ล็อกอิน
                group.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "แก้ไขกลุ่มผู้ใช้งานสำเร็จ";
                return RedirectToPage(new { applicationId = EditGroup.ApplicationId });

            }
            catch (Exception ex)
            {
                TempData["Error"] = "เกิดข้อผิดพลาด: " + ex.Message;
                ViewData["ShowEditModal"] = true;
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
{
    Console.WriteLine("✅ OnPostDeleteAsync เรียกใช้แล้ว");
    try
    {
        var group = await _context.Groups.FindAsync(DeleteId);

        if (group == null)
        {
            TempData["Error"] = "ไม่พบกลุ่มที่ต้องการลบ";
            return RedirectToPage(); // กลับหน้าเดิมแบบไม่ต้องใช้ appId
        }

        var appId = group.ApplicationId;

        // ตรวจสอบว่ามี UserGroup ที่ใช้กลุ่มนี้หรือไม่
        var hasUsers = await _context.UserGroups.AnyAsync(ug => ug.GroupId == DeleteId);
        if (hasUsers)
        {
            TempData["Error"] = "ไม่สามารถลบกลุ่มนี้ได้ เนื่องจากมีผู้ใช้งานอยู่ในกลุ่ม";
            return RedirectToPage(new { applicationId = appId });
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "ลบกลุ่มผู้ใช้งานสำเร็จ";
        return RedirectToPage(new { applicationId = appId });
    }
    catch (Exception ex)
    {
        TempData["Error"] = "เกิดข้อผิดพลาด: " + ex.Message;
        return RedirectToPage(); // เผื่อ group == null แล้ว throw
    }
}


        private async Task LoadDataAsync()
    {
        Applications = await _context.Applications
            .OrderBy(a => a.ApplicationName)
            .ToListAsync();

        // แก้ไขให้รักษา filter ที่เลือกไว้
        var query = _context.Groups.Include(g => g.Application).AsQueryable();
        
        if (SelectedApplicationId.HasValue)
        {
            query = query.Where(g => g.ApplicationId == SelectedApplicationId.Value);
        }

        Groups = await query
            .OrderBy(g => g.GroupName)
            .ToListAsync();
    }
    private void ResetNewGroup()
{
    NewGroup = new GroupCreateModel
    {
        ApplicationId = Guid.Empty,
        GroupName = string.Empty,
        Description = string.Empty
    };
}

    }
    

    // DTO Models สำหรับ Form
    public class GroupCreateModel
    {
        [Required(ErrorMessage = "กรุณาเลือกแอปพลิเคชัน")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อกลุ่ม")]
        [StringLength(100, ErrorMessage = "ชื่อกลุ่มต้องไม่เกิน 100 ตัวอักษร")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "คำอธิบายต้องไม่เกิน 500 ตัวอักษร")]
        public string? Description { get; set; }
    }

    public class GroupEditModel
    {
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกแอปพลิเคชัน")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อกลุ่ม")]
        [StringLength(100, ErrorMessage = "ชื่อกลุ่มต้องไม่เกิน 100 ตัวอักษร")]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "คำอธิบายต้องไม่เกิน 500 ตัวอักษร")]
        public string? Description { get; set; }
    }
}