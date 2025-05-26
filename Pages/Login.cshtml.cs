using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWebApp.Pages;

public class LoginModel : PageModel
{
    public IActionResult OnGet()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/"
        }, "oidc");
    }
}
