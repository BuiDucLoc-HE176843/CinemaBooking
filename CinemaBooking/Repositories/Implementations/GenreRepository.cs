using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<Genre>> GetByMovieIdAsync(int movieId)
        {
            return await _context.MovieGenres
                .Where(x => x.MovieId == movieId)
                .Include(x => x.Genre)
                .Select(x => x.Genre!)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
