namespace CinemaApp.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        public int MovieId { get; set; }

        public DateTime StartTime { get; set; }

        public decimal TicketPrice { get; set; }

        public Movie? Movie { get; set; }

        public List<Seat> Seats { get; set; } = new List<Seat>();

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}