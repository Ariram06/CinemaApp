using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repository
{
    public class BookingRepository
    {
        private readonly AppDbContext _db;

        public BookingRepository(AppDbContext db)
        {
            _db = db;
        }

        // CREATE BOOKING
        public bool CreateBooking(
            int customerId,
            int showId,
            List<int> seatIds)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                Customer? customer = _db.Customers
                    .FirstOrDefault(c => c.CustomerId == customerId);

                Show? show = _db.Shows
                    .FirstOrDefault(s => s.ShowId == showId);

                if (customer == null || show == null ||
                    show.StartTime <= DateTime.Now)
                {
                    return false;
                }

                List<int> uniqueSeatIds = seatIds.Distinct().ToList();

                if (uniqueSeatIds.Count != seatIds.Count ||
                    uniqueSeatIds.Count == 0)
                {
                    return false;
                }

                List<Seat> seats = _db.Seats
                    .Where(s => uniqueSeatIds.Contains(s.SeatId) &&
                                s.ShowId == showId)
                    .ToList();

                if (seats.Count != uniqueSeatIds.Count)
                {
                    return false;
                }

                bool alreadyBooked = _db.BookingSeats.Any(bs =>
                    bs.ShowId == showId &&
                    uniqueSeatIds.Contains(bs.SeatId) &&
                    bs.IsActive);

                if (alreadyBooked)
                {
                    return false;
                }

                Booking booking = new Booking
                {
                    CustomerId = customerId,
                    ShowId = showId,
                    BookingDate = DateTime.Now,
                    TotalAmount = show.TicketPrice * seats.Count,
                    BookingStatus = "Active"
                };

                _db.Bookings.Add(booking);
                _db.SaveChanges();

                foreach (Seat seat in seats)
                {
                    BookingSeat bookingSeat = new BookingSeat
                    {
                        BookingId = booking.BookingId,
                        ShowId = showId,
                        SeatId = seat.SeatId,
                        IsActive = true
                    };

                    _db.BookingSeats.Add(bookingSeat);
                }

                _db.SaveChanges();
                transaction.Commit();

                return true;
            }
            catch (DbUpdateException)
            {
                transaction.Rollback();
                return false;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // VIEW ALL BOOKINGS
        public List<Booking> GetAllBookings()
        {
            return _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Show)
                    .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .AsNoTracking()
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        // SEARCH BOOKING BY ID
        public Booking? GetBookingById(int id)
        {
            return _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Show)
                    .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefault(b => b.BookingId == id);
        }

        // BOOKINGS BY CUSTOMER
        public List<Booking> GetBookingsByCustomer(int customerId)
        {
            return _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Show)
                    .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.CustomerId == customerId)
                .AsNoTracking()
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        // BOOKINGS BY SHOW
        public List<Booking> GetBookingsByShow(int showId)
        {
            return _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Show)
                    .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.ShowId == showId)
                .AsNoTracking()
                .ToList();
        }

        // AVAILABLE SEATS
        public List<Seat> GetAvailableSeats(int showId)
        {
            return _db.Seats
                .Where(s =>
                    s.ShowId == showId &&
                    !_db.BookingSeats.Any(bs =>
                        bs.ShowId == showId &&
                        bs.SeatId == s.SeatId &&
                        bs.IsActive))
                .AsNoTracking()
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToList();
        }

        // CANCEL BOOKING
        public bool CancelBooking(int bookingId)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                Booking? booking = _db.Bookings
                    .Include(b => b.BookingSeats)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null || booking.BookingStatus != "Active")
                {
                    return false;
                }

                booking.BookingStatus = "Cancelled";

                foreach (BookingSeat bookingSeat in booking.BookingSeats)
                {
                    bookingSeat.IsActive = false;
                }

                _db.SaveChanges();
                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}