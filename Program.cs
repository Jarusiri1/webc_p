// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.EntityFrameworkCore;
// using MyWebApp.Data;
// using Microsoft.Extensions.Logging;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddRazorPages(options =>
// {
//     // options.Conventions.AddPageRoute("/Login", "");
//     options.Conventions.AddPageRoute("/wwwroot/js/keycloak", "");
//     // ./wwwroot/js/keycloak
// });

// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//     options.LogTo(Console.WriteLine, LogLevel.Information);
// });

// builder.Services.AddSession(); // ✅ เพิ่ม Service สำหรับ Session

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// // ✅ ใช้ Session ก่อน Routing!
// app.UseSession();

// app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();

// app.Run();


// test use
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.AspNetCore.Authentication;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddRazorPages();

// // 🔐 Keycloak OpenID Connect configuration
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "Cookies";
//     options.DefaultChallengeScheme = "oidc";
// })
// .AddCookie("Cookies")
// .AddOpenIdConnect("oidc", options =>
// {
//     // options.Conventions.AddPageRoute("/Login", "");
//     options.Authority = "https://sso2.pea.co.th/realms/pea-users"; // Keycloak realm URL

//     options.ClientId = "pea-ics";
//     // options.ClientSecret = "YOUR_CLIENT_SECRET"; // from Keycloak
//     options.ClientSecret = "5445e2ac-94fd-4a90-90c4-fa320c8eb2cb"; // from Keycloak

//     options.ResponseType = "code";

//     options.SaveTokens = true;
//     options.GetClaimsFromUserInfoEndpoint = true;

//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         NameClaimType = "preferred_username",
//         RoleClaimType = "roles"
//     };
// });
// builder.WebHost.UseUrls("https://localhost:5286");
// var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();

// app.MapGet("/logout", async context =>
// {
//     await context.SignOutAsync("Cookies");
//     await context.SignOutAsync("oidc", new AuthenticationProperties
//     {
//         RedirectUri = "/App"
//     });
// });

// app.Run();


// test use




// test3 use
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using MyWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Razor Pages (ไม่ต้อง AddPageRoute ซ้ำ)
builder.Services.AddRazorPages();

// ✅ Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// ✅ Session
builder.Services.AddSession();

// ✅ Authentication: Cookie + OpenID Connect (Keycloak)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "OpenIdConnect";
})
.AddCookie("Cookies")
.AddOpenIdConnect("OpenIdConnect", options =>
{
    options.Authority = "https://sso2.pea.co.th/realms/pea-users";
    options.ClientId = "pea-ics";
    options.ClientSecret = "5445e2ac-94fd-4a90-90c4-fa320c8eb2cb";
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };
    options.CallbackPath = "/signin-oidc";
});

builder.WebHost.UseUrls("https://localhost:5286");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Redirect root → /App
app.MapGet("/", context =>
{
    context.Response.Redirect("/App");
    return Task.CompletedTask;
});

// ✅ Map Razor Pages
app.MapRazorPages();

// ✅ Logout endpoint
app.MapGet("/logout", async context =>
{
    await context.SignOutAsync("Cookies");
    await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
    {
        RedirectUri = "/App"
    });
});

app.Run();


// test3

//test4
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Logging;
// using MyWebApp.Data;

// var builder = WebApplication.CreateBuilder(args);

// // ✅ Razor Pages (ไม่ต้อง AddPageRoute ซ้ำ)
// builder.Services.AddRazorPages();

// // ✅ Database Context
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//     options.LogTo(Console.WriteLine, LogLevel.Information);
// });

// // ✅ Session
// builder.Services.AddSession();

// // ✅ Authentication: Cookie + OpenID Connect (Keycloak)
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "Cookies";
//     options.DefaultChallengeScheme = "OpenIdConnect";
// })
// .AddCookie("Cookies")
// .AddOpenIdConnect("OpenIdConnect", options =>
// {
//     options.Authority = "https://sso2.pea.co.th/realms/pea-users";
//     options.ClientId = "pea-ics";
//     options.ClientSecret = "5445e2ac-94fd-4a90-90c4-fa320c8eb2cb";
//     options.ResponseType = "code";
//     options.UsePkce = true;
//     options.SaveTokens = true;
//     options.GetClaimsFromUserInfoEndpoint = true;

//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         NameClaimType = "preferred_username",
//         RoleClaimType = "roles"
//     };

//     // 🔥 Force redirect after login
//     options.Events = new OpenIdConnectEvents
//     {
//         OnTokenValidated = ctx =>
//         {
//             ctx.Response.Redirect("/App");
//             ctx.HandleResponse(); // ← หยุด flow ปกติ เพื่อใช้ redirect ของเรา
//             return Task.CompletedTask;
//         }
//     };

//     options.CallbackPath = "/signin-oidc";
// });



//test4