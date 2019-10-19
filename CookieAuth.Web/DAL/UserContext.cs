using CookieAuth.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CookieAuth.Web.DAL
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> context):base(context)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");
            });
        }
    }
}
