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

        public DbSet<Show> Shows => Set<Show>();

        public DbSet<Seat> Seats => Set<Seat>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=.\SQLEXPRESS;Database=CinemaAppDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MOVIE
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

            // SHOW
            modelBuilder.Entity<Show>(e =>
            {
                e.HasKey(s => s.ShowId);

                e.Property(s => s.TicketPrice)
                    .HasColumnType("decimal(10,2)");

                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] > 0"));

                e.HasOne(s => s.Movie)
                    .WithMany(m => m.Shows)
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SEAT
            modelBuilder.Entity<Seat>(e =>
            {
                e.HasKey(s => s.SeatId);

                e.Property(s => s.RowNumber)
                    .IsRequired()
                    .HasMaxLength(5);

                e.Property(s => s.SeatType)
                    .IsRequired()
                    .HasMaxLength(20);

                e.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Seats_SeatNumber",
                        "[SeatNumber] > 0");

                    t.HasCheckConstraint(
                        "CK_Seats_SeatType",
                        "[SeatType] IN ('Regular', 'Premium')");
                });

                e.HasIndex(s => new
                {
                    s.ShowId,
                    s.RowNumber,
                    s.SeatNumber
                }).IsUnique();

                e.HasOne(s => s.Show)
                    .WithMany(sh => sh.Seats)
                    .HasForeignKey(s => s.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CUSTOMER
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.CustomerId);

                e.Property(c => c.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                e.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                e.HasIndex(c => c.Email).IsUnique();
            });

            // BOOKING
            modelBuilder.Entity<Booking>(e =>
            {
                e.HasKey(b => b.BookingId);

                e.Property(b => b.TotalAmount)
                    .HasColumnType("decimal(10,2)");

                e.Property(b => b.BookingStatus)
                    .IsRequired()
                    .HasMaxLength(20);

                e.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Bookings_TotalAmount",
                        "[TotalAmount] >= 0");

                    t.HasCheckConstraint(
                        "CK_Bookings_Status",
                        "[BookingStatus] IN ('Active', 'Cancelled')");
                });

                e.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Show)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BOOKING SEAT
            modelBuilder.Entity<BookingSeat>(e =>
            {
                e.HasKey(bs => bs.BookingSeatId);

                e.HasOne(bs => bs.Booking)
                    .WithMany(b => b.BookingSeats)
                    .HasForeignKey(bs => bs.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(bs => bs.Seat)
                    .WithMany()
                    .HasForeignKey(bs => bs.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                // A seat can have only one active booking per show.
                e.HasIndex(bs => new
                {
                    bs.ShowId,
                    bs.SeatId
                })
                .IsUnique()
                .HasFilter("[IsActive] = 1");
            });
        }
    }
}