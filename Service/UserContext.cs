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

        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Пример: Конфигурация отношения многие-ко-многим между User и Role
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("Roles"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
