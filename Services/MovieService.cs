using CinemaApp.Models;
using CinemaApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services
{
    public class MovieService
    {
        private readonly MovieRepository _repository;
        private readonly AuditLogService _auditLogService;

        public MovieService(MovieRepository repository)
            : this(repository, new AuditLogService())
        {
        }

        public MovieService(MovieRepository repository, AuditLogService auditLogService)
        {
            _repository = repository;
            _auditLogService = auditLogService;
        }

        // ADD MOVIE
        public void AddMovie()
        {
            Movie movie = new Movie();

            Console.Write("Enter movie title: ");
            movie.Title = Console.ReadLine() ?? "";

            Console.Write("Enter movie genre: ");
            movie.Genre = Console.ReadLine() ?? "";

            Console.Write("Enter movie duration: ");

            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Invalid duration!");
                return;
            }

            movie.DurationMinutes = duration;

            if (string.IsNullOrWhiteSpace(movie.Title) ||
                string.IsNullOrWhiteSpace(movie.Genre))
            {
                Console.WriteLine("Title and genre are required!");
                return;
            }

            if (movie.Title.Length > 100 || movie.Genre.Length > 100)
            {
                Console.WriteLine("Title or genre is too long!");
                return;
            }

            if (duration <= 0)
            {
                Console.WriteLine("Duration must be greater than zero!");
                return;
            }

            _repository.AddMovie(movie);

            _auditLogService.Log("Movie", "Added", $"Movie ID: {movie.MovieId} | Movie: {movie.Title}");

            Console.WriteLine("Movie added successfully!");
        }

        // VIEW ALL MOVIES
        public void ViewMovies()
        {
            List<Movie> movies = _repository.GetAllMovies();

            if (movies.Count == 0)
            {
                Console.WriteLine("No movies available!");
                return;
            }

            foreach (var movie in movies)
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"ID       : {movie.MovieId}");
                Console.WriteLine($"Title    : {movie.Title}");
                Console.WriteLine($"Genre    : {movie.Genre}");
                Console.WriteLine($"Duration : {movie.DurationMinutes} minutes");
            }
        }

        // SEARCH BY ID
        public void SearchMovieById()
        {
            Console.Write("Enter movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid ID!");
                return;
            }

            Movie? movie = _repository.GetMovieById(id);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            Console.WriteLine($"ID       : {movie.MovieId}");
            Console.WriteLine($"Title    : {movie.Title}");
            Console.WriteLine($"Genre    : {movie.Genre}");
            Console.WriteLine($"Duration : {movie.DurationMinutes} minutes");
        }

        // SEARCH BY TITLE
        public void SearchMovieByTitle()
        {
            Console.Write("Enter movie title: ");

            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title cannot be empty!");
                return;
            }

            List<Movie> movies = _repository.SearchMovie(title.Trim());

            if (movies.Count == 0)
            {
                Console.WriteLine("No matching movies found!");
                return;
            }

            foreach (var movie in movies)
            {
                Console.WriteLine(
                    $"{movie.MovieId} | {movie.Title} | {movie.Genre} | {movie.DurationMinutes} minutes");
            }
        }

        // UPDATE MOVIE
        public void UpdateMovie()
        {
            Console.Write("Enter movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid ID!");
                return;
            }

            Movie? movie = _repository.GetMovieById(id);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            Console.Write("Enter new title: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Enter new genre: ");
            string genre = Console.ReadLine() ?? "";

            Console.Write("Enter new duration: ");

            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Invalid duration!");
                return;
            }

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(genre) ||
                title.Length > 100 ||
                genre.Length > 100 ||
                duration <= 0)
            {
                Console.WriteLine("Invalid movie details!");
                return;
            }

            movie.Title = title.Trim();
            movie.Genre = genre.Trim();
            movie.DurationMinutes = duration;

            _repository.UpdateMovie(movie);

            _auditLogService.Log("Movie", "Updated", $"Movie ID: {movie.MovieId} | Movie: {movie.Title}");

            Console.WriteLine("Movie updated successfully!");
        }

        // DELETE MOVIE
        public void DeleteMovie()
        {
            Console.Write("Enter movie ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid ID!");
                return;
            }

            Movie? movie = _repository.GetMovieById(id);

            if (movie == null)
            {
                Console.WriteLine("Movie not found!");
                return;
            }

            Console.Write("Are you sure? (y/n): ");

            string confirm = Console.ReadLine() ?? "";

            if (confirm.ToLower() == "y")
            {
                _repository.DeleteMovie(movie);
                _auditLogService.Log("Movie", "Deleted", $"Movie ID: {movie.MovieId} | Movie: {movie.Title}");
                Console.WriteLine("Movie deleted successfully!");
            }
            else
            {
                Console.WriteLine("Delete cancelled!");
            }
        }

        // MOVIE MENU
        public void Run()
        {
            bool valid = true;

            while (valid)
            {
                Console.WriteLine("\n========== MOVIE MANAGEMENT ==========");
                Console.WriteLine("1. Add Movie");
                Console.WriteLine("2. View All Movies");
                Console.WriteLine("3. Search Movie By ID");
                Console.WriteLine("4. Search Movie By Title");
                Console.WriteLine("5. Update Movie");
                Console.WriteLine("6. Delete Movie");
                Console.WriteLine("0. Back to Main Menu");

                Console.Write("\nEnter your choice: ");

                string choice = Console.ReadLine() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddMovie();
                            break;

                        case "2":
                            ViewMovies();
                            break;

                        case "3":
                            SearchMovieById();
                            break;

                        case "4":
                            SearchMovieByTitle();
                            break;

                        case "5":
                            UpdateMovie();
                            break;

                        case "6":
                            DeleteMovie();
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