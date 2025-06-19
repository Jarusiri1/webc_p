using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        public LoginModel(AppDbContext context) => _context = context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "รหัสพนักงานต้องเป็นตัวเลข 6 หลัก")]
        public string EmployeeNo { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == EmployeeNo);
            if (emp == null)
            {
                Console.WriteLine("❌ ไม่เจอพนักงาน");
                ErrorMessage = "ไม่พบรหัสพนักงานนี้ในระบบ";
                return Page();
            }
            Console.WriteLine("❌ ไม่เจอพนักงาน");

            HttpContext.Session.SetString("EmployeeNo", emp.EmployeeNo);
            HttpContext.Session.SetString("FullName", emp.FullName ?? "");

            Console.WriteLine("✅ Login success, redirecting to /App");

            return RedirectToPage("/App");
        }
    }
}
