namespace CinemaApp.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int ShowId { get; set; }

        public string RowNumber { get; set; } = "";

        public int SeatNumber { get; set; }

        public string SeatType { get; set; } = "";

        public Show? Show { get; set; }
    }
}