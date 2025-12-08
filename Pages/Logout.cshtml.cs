using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace MyWebApp.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // ✅ Clear local cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ Sign out from Keycloak (redirects there)
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                // RedirectUri = Url.Page("/Index") // where to go after logout
                RedirectUri = Url.Page("/Login") // where to go after logout
            });

            // return RedirectToPage("/Index");
            return RedirectToPage("/Login");

        }
    }
}
