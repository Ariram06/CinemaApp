using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repository
{
    public class SeatRepository
    {
        private readonly AppDbContext _db;

        public SeatRepository(AppDbContext db)
        {
            _db = db;
        }

        // ADD SEAT
        public void AddSeat(Seat seat)
        {
            _db.Seats.Add(seat);
            _db.SaveChanges();
        }

        // VIEW ALL SEATS
        public List<Seat> GetAllSeats()
        {
            return _db.Seats
                .Include(s => s.Show)
                    .ThenInclude(sh => sh!.Movie)
                .AsNoTracking()
                .ToList();
        }

        // SEARCH SEAT BY ID
        public Seat? GetSeatById(int id)
        {
            return _db.Seats
                .Include(s => s.Show)
                .FirstOrDefault(s => s.SeatId == id);
        }

        // VIEW SEATS BY SHOW
        public List<Seat> GetSeatsByShow(int showId)
        {
            return _db.Seats
                .AsNoTracking()
                .Where(s => s.ShowId == showId)
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .ToList();
        }

        // CHECK DUPLICATE SEAT POSITION
        public bool IsSeatPositionTaken(
            int showId,
            string rowNumber,
            int seatNumber,
            int excludeSeatId = 0)
        {
            return _db.Seats.Any(s =>
                s.ShowId == showId &&
                s.RowNumber == rowNumber &&
                s.SeatNumber == seatNumber &&
                s.SeatId != excludeSeatId);
        }

        // UPDATE SEAT
        public void UpdateSeat(Seat seat)
        {
            _db.Seats.Update(seat);
            _db.SaveChanges();
        }

        // DELETE SEAT
        public void DeleteSeat(Seat seat)
        {
            _db.Seats.Remove(seat);
            _db.SaveChanges();
        }
    }
}