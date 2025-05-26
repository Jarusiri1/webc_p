using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data  // เพิ่ม namespace นี้
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; } // ถูกต้องแล้ว
    }
}
