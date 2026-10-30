using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repository
{
    public class ShowRepository
    {
        private readonly AppDbContext _db;

        public ShowRepository(AppDbContext db)
        {
            _db = db;
        }

        // ADD SHOW
        public void AddShow(Show show)
        {
            _db.Shows.Add(show);
            _db.SaveChanges();
        }

        // VIEW ALL SHOWS
        public List<Show> GetAllShows()
        {
            return _db.Shows
                .Include(s => s.Movie)
                .AsNoTracking()
                .ToList();
        }

        // SEARCH SHOW BY ID
        public Show? GetShowById(int id)
        {
            return _db.Shows
                .Include(s => s.Movie)
                .FirstOrDefault(s => s.ShowId == id);
        }

        // GET SHOWS BY MOVIE ID
        public List<Show> GetShowsByMovie(int movieId)
        {
            return _db.Shows
                .Include(s => s.Movie)
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .ToList();
        }

        // UPDATE SHOW
        public void UpdateShow(Show show)
        {
            _db.Shows.Update(show);
            _db.SaveChanges();
        }

        // DELETE SHOW
        public void DeleteShow(Show show)
        {
            _db.Shows.Remove(show);
            _db.SaveChanges();
        }
    }
}