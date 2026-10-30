using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repository
{
    public class MovieRepository
    {
        private readonly AppDbContext _db;

        public MovieRepository(AppDbContext db)
        {
            _db = db;
        }

        public void AddMovie(Movie movie)
        {
            _db.Movies.Add(movie);
            _db.SaveChanges();
        }

        public List<Movie> GetAllMovies()
        {
            return _db.Movies.AsNoTracking().ToList();
        }

        public Movie? GetMovieById(int id)
        {
            return _db.Movies.FirstOrDefault(m => m.MovieId == id);
        }

        public List<Movie> SearchMovie(string title)
        {
            return _db.Movies
                .AsNoTracking()
                .Where(m => m.Title.Contains(title))
                .ToList();
        }

        public void UpdateMovie(Movie movie)
        {
            _db.Movies.Update(movie);
            _db.SaveChanges();
        }

        public void DeleteMovie(Movie movie)
        {
            _db.Movies.Remove(movie);
            _db.SaveChanges();
        }
    }
}