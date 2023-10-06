using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UsersApi.Model;

namespace UsersApi.Service
{    /// <summary>
     /// класс контекст подключения к базе данных
     /// </summary>
    public class UserContext : DbContext
    {
        /// <summary>
        /// констурктор
        /// </summary>
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        /// <summary>
        /// данные таблицы users
        /// </summary>
        public DbSet<User>? users { get; set; }
        /// <summary>
        /// данные таблицы roles
        /// </summary>
        public DbSet<Role>? roles { get; set; }
        /// <summary>
        /// данные промежуточной таблицы userRoles
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// создание модели на основе зависимостей 
        /// </summary>
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
