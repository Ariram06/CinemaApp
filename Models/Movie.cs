namespace CinemaApp.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; } = "";

        public string Genre { get; set; } = "";

        public int DurationMinutes { get; set; }

        public List<Show> Shows { get; set; } = new List<Show>();
    }
}