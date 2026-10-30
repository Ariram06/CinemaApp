namespace CinemaApp.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}