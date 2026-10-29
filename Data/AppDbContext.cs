using Microsoft.EntityFrameworkCore;
using CinemaApp.Models;

namespace CinemaApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies => Set<Movie>();

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=.\SQLEXPRESS;Database=CinemaAppDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>(e =>
            {
                e.HasKey(m => m.MovieId);

                e.Property(m => m.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(m => m.Genre)
                    .IsRequired()
                    .HasMaxLength(100);

                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Movies_Duration",
                    "[DurationMinutes] > 0"));
            });
        }
    }
}