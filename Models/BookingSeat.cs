namespace CinemaApp.Models
{
    public class BookingSeat
    {
        public int BookingSeatId { get; set; }

        public int BookingId { get; set; }

        public int ShowId { get; set; }

        public int SeatId { get; set; }

        public bool IsActive { get; set; } = true;

        public Booking? Booking { get; set; }

        public Seat? Seat { get; set; }
    }
}