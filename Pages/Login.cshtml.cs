using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
        public string EmployeeNo { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            TempData["EmployeeNo"] = EmployeeNo;
            return RedirectToPage("/App");
        }
    }
}
