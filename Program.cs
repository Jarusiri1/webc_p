using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ✅ เพิ่ม Razor Pages และกำหนดให้ "/Login" เป็นหน้าแรก (route "/")
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Login", ""); // localhost:xxxx/ → Login
});

// ✅ ตั้งค่า Entity Framework Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));

    // ✅ แสดง SQL Query ใน Console
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// 🔐 Authentication (ยังไม่เปิดใช้ เพราะยังใช้ TempData อยู่)
// ถ้าใช้ Cookie/Identity จริง ค่อยเปิดส่วนนี้
/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(...);
*/

var app = builder.Build();

// ✅ Middleware: Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 Auth Middleware (เผื่อใช้ในอนาคต)
app.UseAuthentication();
app.UseAuthorization();

// ✅ Razor Pages Routes
app.MapRazorPages();

// ⛔ ไม่ต้องใช้ fallback ไป Login อีกแล้ว
// app.MapFallbackToPage("/Login");

app.Run();
