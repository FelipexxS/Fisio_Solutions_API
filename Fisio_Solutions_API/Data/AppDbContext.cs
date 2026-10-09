using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Profession).HasMaxLength(150);
                entity.Property(u => u.BirthDate).HasMaxLength(20);
                entity.Property(u => u.PasswordHash).IsRequired();
            });
        }
    }
}

