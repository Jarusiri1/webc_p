using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWebApp.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            TempData.Clear(); // ล้างข้อมูล login
            return RedirectToPage("/Login"); // กลับไปหน้า Login
        }
    }
}
