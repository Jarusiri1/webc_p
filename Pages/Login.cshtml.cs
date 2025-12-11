using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace MyWebApp.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }

            // ไป SSO ทันที
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }
    }
}
