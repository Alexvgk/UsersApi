using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UsersApi.Model;

namespace UsersApi.Service
{
    public class UserContext : DbContext
    {

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User>? users { get; set; }
        public DbSet<Role>? roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.roleId, ur.userId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.user)
                .WithMany(u => u.userRoles)
                .HasForeignKey(ur => ur.userId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.role)
                .WithMany(r => r.userRole)
                .HasForeignKey(ur => ur.roleId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
