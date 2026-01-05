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
public GroupModel(AppDbContext context){_context=context;}


public List<Group> Groups { get; set; } = new();
public List<Application> Applications { get; set; } = new();
public Guid? SelectedApplicationId { get; set; }


[BindProperty] public GroupCreateModel NewGroup { get; set; } = new();
[BindProperty] public Guid DeleteId { get; set; }


public async Task OnGetAsync(Guid? applicationId)
{
await LoadDataAsync(applicationId);
}


public async Task<IActionResult> OnPostCreateAsync()
{
await LoadDataAsync(NewGroup.ApplicationId);
if (!ModelState.IsValid)
{
return Page();
}


var exists = await _context.Groups.AnyAsync(g =>
g.GroupName == NewGroup.GroupName && g.ApplicationId == NewGroup.ApplicationId);
if (exists)
{
ModelState.AddModelError("NewGroup.GroupName","ชื่อกลุ่มซ้ำ");
return Page();
}


_context.Groups.Add(new Group{
GroupId = Guid.NewGuid(),
ApplicationId = NewGroup.ApplicationId,
GroupName = NewGroup.GroupName,
Description = NewGroup.Description,
CreateDate = DateTime.Now,
UpdateDate = DateTime.Now,
CreateBy = "System",
UpdateBy = "System"
});
await _context.SaveChangesAsync();
TempData["SuccessMessage"] = "เพิ่มกลุ่มสำเร็จ";
return RedirectToPage(new{applicationId=NewGroup.ApplicationId});
}


public async Task<IActionResult> OnPostDeleteAsync()
{
var g = await _context.Groups.FindAsync(DeleteId);
if (g!=null){_context.Groups.Remove(g);await _context.SaveChangesAsync();}
TempData["SuccessMessage"] = "ลบกลุ่มสำเร็จ";
return RedirectToPage();
}


private async Task LoadDataAsync(Guid? applicationId)
{
SelectedApplicationId = applicationId;
Applications = await _context.Applications.OrderBy(a=>a.ApplicationName).ToListAsync();
var q = _context.Groups.Include(g=>g.Application).AsQueryable();
if(applicationId.HasValue) q=q.Where(g=>g.ApplicationId==applicationId);
Groups = await q.OrderBy(g=>g.GroupName).ToListAsync();
}
}


public class GroupCreateModel
{
[Required] public Guid ApplicationId { get; set; }
[Required,StringLength(100)] public string GroupName { get; set; } = string.Empty;
[StringLength(500)] public string? Description { get; set; }
}
}