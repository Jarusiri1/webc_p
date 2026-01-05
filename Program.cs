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
//     options.CallbackPath = "/signin-oidc";
// });

// builder.WebHost.UseUrls("https://localhost:5286");

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseSession();
// app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

// // ✅ Redirect root → /App
// app.MapGet("/", context =>
// {
//     context.Response.Redirect("/App");
//     return Task.CompletedTask;
// });

// // ✅ Map Razor Pages
// app.MapRazorPages();

// // ✅ Logout endpoint
// app.MapGet("/logout", async context =>
// {
//     await context.SignOutAsync("Cookies");
//     await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
//     {
//         RedirectUri = "/App"
//     });
// });

// app.Run();


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

// // Razor Pages
// builder.Services.AddRazorPages(options =>
// {
//     // Require Auth for /App
//     options.Conventions.AuthorizeFolder("/App");
// });

// // Database
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//     options.LogTo(Console.WriteLine, LogLevel.Information);
// });

// // Session
// builder.Services.AddSession();

// // Authentication
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "Cookies";
//     options.DefaultChallengeScheme = "OpenIdConnect";  // แก้ให้ถูกต้อง
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

//     // token mapping
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         NameClaimType = "preferred_username",
//         RoleClaimType = "roles"
//     };

//     options.CallbackPath = "/signin-oidc";

//     // เมื่อ login เสร็จ → ไป /App
//     options.Events = new OpenIdConnectEvents
//     {
//         OnTokenValidated = context =>
//         {
//             context.Response.Redirect("/App");
//             context.HandleResponse();
//             return Task.CompletedTask;
//         }
//     };
// });

// builder.WebHost.UseUrls("https://localhost:5286");

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseSession();
// app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

// // Redirect root → /App
// app.MapGet("/", context =>
// {
//     context.Response.Redirect("/App");
//     return Task.CompletedTask;
// });

// // Razor pages
// app.MapRazorPages();

// // Logout
// app.MapGet("/logout", async context =>
// {
//     await context.SignOutAsync("Cookies");
//     await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
//     {
//         RedirectUri = "/"
//     });
// });

// app.Run();



//test4


// test5 redirect to index page for login keycloack ****** ใช้อยู่ตอนนี้
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
//     options.CallbackPath = "/signin-oidc";
// });

// builder.WebHost.UseUrls("https://localhost:5286");

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseSession();
// app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

// // ✅ Redirect root → /App
// app.MapGet("/", context =>
// {
//     context.Response.Redirect("/Index");
//     return Task.CompletedTask;
// });

// // ✅ Map Razor Pages
// app.MapRazorPages();

// // ✅ Logout endpoint
// app.MapGet("/logout", async context =>
// {
//     await context.SignOutAsync("Cookies");
//     await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
//     {
//         RedirectUri = "/Index"
//     });
// });

// app.Run();


// test6 ****
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using MyWebApp.Data;
// using Microsoft.AspNetCore.HttpOverrides;

// var builder = WebApplication.CreateBuilder(args);

// // -------------------------------
// // Razor Pages
// // -------------------------------
// builder.Services.AddRazorPages();

// // -------------------------------
// // Database
// // -------------------------------
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

// // -------------------------------
// // Force HTTPS port for OIDC Redirect (สำคัญมาก!)
// // -------------------------------
// builder.WebHost.UseUrls("https://localhost:5286", "http://localhost:5285");

// // -------------------------------
// // Authentication
// // -------------------------------
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "Cookies";
//     options.DefaultChallengeScheme = "oidc";
// })
// .AddCookie("Cookies", options =>
// {
//     options.LoginPath = "/Login";
//     options.LogoutPath = "/Logout";
// })
// .AddOpenIdConnect("oidc", options =>
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

//     // Callback ต้องตรงกับ Keycloak
//     options.CallbackPath = "/signin-oidc";

//     // Logout callback
//     options.SignedOutCallbackPath = "/signout-callback-oidc";

//     // กำหนด RedirectUri ชัดเจน (กัน error https port)
//     options.Events = new OpenIdConnectEvents
//     {
//         OnRedirectToIdentityProvider = context =>
//         {
//             context.ProtocolMessage.RedirectUri = "https://localhost:5286/signin-oidc";
//             return Task.CompletedTask;
//         }
//     };
// });

// // -------------------------------
// // Authorization - บังคับ Login ทุกหน้า
// // -------------------------------
// builder.Services.AddAuthorization(options =>
// {
//     options.FallbackPolicy = options.DefaultPolicy;
// });

// // -------------------------------
// // Build App
// // -------------------------------
// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// // -------------------------------
// // Fix reverse-proxy / redirect port issues
// // -------------------------------
// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedProto
// });

// app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();

// // -------------------------------
// // Logout
// // -------------------------------
// app.MapGet("/Logout", async context =>
// {
//     await context.SignOutAsync("Cookies");

//     await context.SignOutAsync("oidc", new AuthenticationProperties
//     {
//         RedirectUri = "/"
//     });
// });

// // -------------------------------
// app.Run();


// test6

// test7
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.IdentityModel.Tokens;

// var builder = WebApplication.CreateBuilder(args);

// // ---------- Add Razor & Auth ----------
// builder.Services.AddRazorPages(options =>
// {
//     // ❗ ทุกหน้าต้อง Login (ยกเว้น Login Page)
//     options.Conventions.AuthorizeFolder("/");
//     options.Conventions.AllowAnonymousToPage("/Login");
// });

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
// })
// .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
// .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
// {
//     options.Authority = "https://sso2.pea.co.th/realms/pea-users"; 
//     options.ClientId = "pea-ics";
//     options.ClientSecret = "5445e2ac-94fd-4a90-90c4-fa320c8eb2cb";
//     options.ResponseType = "code";

//     options.SaveTokens = true;

//     options.GetClaimsFromUserInfoEndpoint = true;

//     // ---------- Callback ----------
//     options.CallbackPath = "/signin-oidc";

//     // ---------- Logout ----------
//     options.SignedOutRedirectUri = "/";

//     // ง่ายขึ้น (SSO ส่วนมากรองรับ)
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = false
//     };

//     // ปิดปัญหา HTTPS port error
//     options.RequireHttpsMetadata = false;
// });

// var app = builder.Build();

// // ---------- HTTPS / Static / Routing ----------
// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseRouting();

// // ---------- Authentication / Authorization ----------
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapRazorPages();

// app.Run();



// test7

// test8
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using MyWebApp.Data;

// var builder = WebApplication.CreateBuilder(args);

// // -------------------------------
// // Razor Pages
// // -------------------------------
// builder.Services.AddRazorPages(options =>
// {
//     // บังคับ login ทุกหน้า
//     options.Conventions.AuthorizeFolder("/");
// });

// // -------------------------------
// // Database
// // -------------------------------
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

// // -------------------------------
// // Session (⭐ สำคัญมาก)
// // -------------------------------
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });

// // -------------------------------
// // Authentication
// // -------------------------------
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
// })
// .AddCookie()
// .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
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

//     options.CallbackPath = "/signin-oidc";
// });

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();

// // ⭐ ลำดับห้ามสลับ
// app.UseSession();
// app.UseAuthentication();
// app.UseAuthorization();

// // -------------------------------
// // Redirect root → /App
// // -------------------------------
// app.MapGet("/", context =>
// {
//     context.Response.Redirect("/App");
//     return Task.CompletedTask;
// });

// // Razor Pages
// app.MapRazorPages();

// // -------------------------------
// // Logout
// // -------------------------------
// app.MapGet("/Logout", async context =>
// {
//     await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

//     await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
//         new AuthenticationProperties
//         {
//             RedirectUri = "/"
//         });
// });

// app.Run();


//test8

//test9
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using MyWebApp.Data;

// var builder = WebApplication.CreateBuilder(args);

// // --------------------
// // Razor Pages
// // --------------------
// builder.Services.AddRazorPages(options =>
// {
//     // 🔐 บังคับทุก Razor Page ต้อง Login
//     options.Conventions.AuthorizeFolder("/");
// });

// // --------------------
// // Database
// // --------------------
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

// // --------------------
// // Session (ถ้าใช้ HttpContext.Session)
// // --------------------
// builder.Services.AddSession();

// // --------------------
// // Authentication
// // --------------------
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
// })
// .AddCookie()
// .AddOpenIdConnect(options =>
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

//     options.CallbackPath = "/signin-oidc";
// });

// // --------------------
// // Authorization
// // --------------------
// builder.Services.AddAuthorization();

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();

// app.UseSession();

// app.UseAuthentication();
// app.UseAuthorization();

// // --------------------
// // ROOT → บังคับ Login
// // --------------------
// app.MapGet("/", async context =>
// {
//     if (!context.User.Identity?.IsAuthenticated ?? true)
//     {
//         await context.ChallengeAsync(
//             OpenIdConnectDefaults.AuthenticationScheme,
//             new AuthenticationProperties { RedirectUri = "/App" }
//         );
//         return;
//     }

//     context.Response.Redirect("/App");
// });

// // --------------------
// // Razor Pages
// // --------------------
// app.MapRazorPages();

// // --------------------
// // Logout
// // --------------------
// app.MapGet("/Logout", async context =>
// {
//     await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

//     await context.SignOutAsync(
//         OpenIdConnectDefaults.AuthenticationScheme,
//         new AuthenticationProperties
//         {
//             RedirectUri = "/"
//         });
// });

// app.Run();




//test9


// test10
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Razor Pages
// ----------------------------
builder.Services.AddRazorPages();

// ----------------------------
// Database
// ----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ----------------------------
// Session (ถ้าใช้ HttpContext.Session)
// ----------------------------
builder.Services.AddSession();

// ----------------------------
// Authentication
// ----------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
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

// ----------------------------
// Authorization (บังคับทุกหน้า Login)
// ----------------------------
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// Root → App
// ----------------------------
app.MapGet("/", context =>
{
    context.Response.Redirect("/App");
    return Task.CompletedTask;
});

// ----------------------------
// Razor Pages
// ----------------------------
app.MapRazorPages();

// ----------------------------
// Logout
// ----------------------------
app.MapGet("/Logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    await context.SignOutAsync(
        OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties
        {
            RedirectUri = "/"
        });
});

app.Run();

//test10