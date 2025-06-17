using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Group>     Groups     { get; set; }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var entries = ChangeTracker
                .Entries<Application>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                }
                entry.Entity.UpdatedDate = DateTime.Now;
            }
        }
        public DbSet<ApplicationAdmin> ApplicationAdmins { get; set; }

    }
}
