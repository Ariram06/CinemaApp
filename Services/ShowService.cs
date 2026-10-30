using CinemaApp.Models;
using CinemaApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services
{
    public class ShowService
    {
        private readonly ShowRepository _showRepository;
        private readonly MovieRepository _movieRepository;
        private readonly AuditLogService _auditLogService;

        public ShowService(
            ShowRepository showRepository,
            MovieRepository movieRepository)
            : this(showRepository, movieRepository, new AuditLogService())
        {
        }

        public ShowService(
            ShowRepository showRepository,
            MovieRepository movieRepository,
            AuditLogService auditLogService)
        {
            _showRepository = showRepository;
            _movieRepository = movieRepository;
            _auditLogService = auditLogService;
        }

        // ADD SHOW
        public void AddShow()
        {
            Console.Write("Enter movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int movieId) ||
                movieId <= 0)
            {
                Console.WriteLine("Invalid movie ID!");
                return;
            }

            Movie? movie = _movieRepository.GetMovieById(movieId);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            Console.Write("Enter show date and time (dd-MM-yyyy HH:mm): ");

            if (!DateTime.TryParseExact(
                Console.ReadLine(),
                "dd-MM-yyyy HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime startTime))
            {
                Console.WriteLine("Invalid date or time!");
                return;
            }

            if (startTime <= DateTime.Now)
            {
                Console.WriteLine("Show time must be in the future!");
                return;
            }

            Console.Write("Enter ticket price: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price) ||
                price <= 0)
            {
                Console.WriteLine("Invalid ticket price!");
                return;
            }

            Show show = new Show();

            show.MovieId = movieId;
            show.StartTime = startTime;
            show.TicketPrice = price;

            _showRepository.AddShow(show);

            _auditLogService.Log("Show", "Added", $"Show ID: {show.ShowId} | Movie ID: {show.MovieId}");

            Console.WriteLine("Show scheduled successfully!");
        }


        // VIEW ALL SHOWS
        public void ViewAllShows()
        {
            List<Show> shows = _showRepository.GetAllShows();

            if (shows.Count == 0)
            {
                Console.WriteLine("No shows available!");
                return;
            }

            foreach (var show in shows)
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Show ID     : {show.ShowId}");
                Console.WriteLine($"Movie       : {show.Movie?.Title}");
                Console.WriteLine($"Movie ID    : {show.MovieId}");
                Console.WriteLine($"Start Time  : {show.StartTime}");
                Console.WriteLine($"Ticket Price: {show.TicketPrice}");
            }
        }


        // SEARCH SHOW BY ID
        public void SearchShowById()
        {
            Console.Write("Enter show ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(id);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            Console.WriteLine($"Show ID     : {show.ShowId}");
            Console.WriteLine($"Movie       : {show.Movie?.Title}");
            Console.WriteLine($"Start Time  : {show.StartTime}");
            Console.WriteLine($"Ticket Price: {show.TicketPrice}");
        }


        // VIEW SHOWS BY MOVIE
        public void ViewShowsByMovie()
        {
            Console.Write("Enter movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int movieId) ||
                movieId <= 0)
            {
                Console.WriteLine("Invalid movie ID!");
                return;
            }

            Movie? movie = _movieRepository.GetMovieById(movieId);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            List<Show> shows = _showRepository.GetShowsByMovie(movieId);

            if (shows.Count == 0)
            {
                Console.WriteLine("No shows scheduled for this movie!");
                return;
            }

            foreach (var show in shows)
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Show ID    : {show.ShowId}");
                Console.WriteLine($"Movie      : {movie.Title}");
                Console.WriteLine($"Start Time : {show.StartTime}");
                Console.WriteLine($"Price      : {show.TicketPrice}");
            }
        }


        // UPDATE SHOW
        public void UpdateShow()
        {
            Console.Write("Enter show ID to update: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(id);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            Console.Write("Enter new movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int movieId) ||
                movieId <= 0)
            {
                Console.WriteLine("Invalid movie ID!");
                return;
            }

            Movie? movie = _movieRepository.GetMovieById(movieId);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            Console.Write("Enter new show date and time (dd-MM-yyyy HH:mm): ");

            if (!DateTime.TryParseExact(
                Console.ReadLine(),
                "dd-MM-yyyy HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime startTime))
            {
                Console.WriteLine("Invalid date or time!");
                return;
            }

            if (startTime <= DateTime.Now)
            {
                Console.WriteLine("Show time must be in the future!");
                return;
            }

            Console.Write("Enter new ticket price: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price) ||
                price <= 0)
            {
                Console.WriteLine("Invalid ticket price!");
                return;
            }

            show.MovieId = movieId;
            show.StartTime = startTime;
            show.TicketPrice = price;

            _showRepository.UpdateShow(show);

            _auditLogService.Log("Show", "Updated", $"Show ID: {show.ShowId}");

            Console.WriteLine("Show updated successfully!");
        }


        // DELETE SHOW
        public void DeleteShow()
        {
            Console.Write("Enter show ID to delete: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(id);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            Console.Write("Are you sure you want to delete this show? (y/n): ");

            string confirm = Console.ReadLine() ?? "";

            if (confirm.ToLower() == "y")
            {
                _showRepository.DeleteShow(show);
                _auditLogService.Log("Show", "Deleted", $"Show ID: {show.ShowId}");

                Console.WriteLine("Show deleted successfully!");
            }
            else
            {
                Console.WriteLine("Delete cancelled!");
            }
        }


        // SHOW MANAGEMENT MENU
        public void Run()
        {
            bool valid = true;

            while (valid)
            {
                Console.WriteLine("\n========== SHOW MANAGEMENT ==========");
                Console.WriteLine("1. Add Show");
                Console.WriteLine("2. View All Shows");
                Console.WriteLine("3. Search Show By ID");
                Console.WriteLine("4. View Shows By Movie");
                Console.WriteLine("5. Update Show");
                Console.WriteLine("6. Delete Show");
                Console.WriteLine("0. Back to Main Menu");

                Console.Write("\nEnter your choice: ");

                string choice = Console.ReadLine() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddShow();
                            break;

                        case "2":
                            ViewAllShows();
                            break;

                        case "3":
                            SearchShowById();
                            break;

                        case "4":
                            ViewShowsByMovie();
                            break;

                        case "5":
                            UpdateShow();
                            break;

                        case "6":
                            DeleteShow();
                            break;

                        case "0":
                            valid = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice!");
                            break;
                    }
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("Database operation failed!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}