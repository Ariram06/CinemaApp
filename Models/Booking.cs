namespace CinemaApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int CustomerId { get; set; }

        public int ShowId { get; set; }

        public DateTime BookingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; } = "Active";

        public Customer? Customer { get; set; }

        public Show? Show { get; set; }

        public List<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}