using CinemaBooking.Configuration;
using CinemaBooking.Data;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GenreExistsAsync(int genreId)
        {
            return await _context.Genres.AnyAsync(x => x.Id == genreId);
        }

        public async Task<bool> IdExistsAsync(int Id)
        {
            return await _context.Movies.AnyAsync(x => x.Id == Id);
        }

        public async Task<PagedResult<Movie>> GetPagedAsync(
            int? id,
            string? title,
            DateTime? releaseDate,
            MovieStatus? status,
            int? genreId,
            bool? IsMainFeature,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Movies
                .Include(x => x.MovieGenres)!
                    .ThenInclude(mg => mg.Genre)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id);

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(x => x.Title.Contains(title));

            if (releaseDate.HasValue)
                query = query.Where(x => x.ReleaseDate >= releaseDate);

            if (status.HasValue)
                query = query.Where(x => x.Status == status);

            if (IsMainFeature.HasValue)
            {
                if (IsMainFeature == true)
                {
                    query = query.Where(x => x.IsMainFeature == true);
                }
                else
                {
                    query = query.Where(x => x.IsMainFeature == false);
                }
            }

            if (genreId.HasValue)
                query = query.Where(x => x.MovieGenres!
                    .Any(mg => mg.GenreId == genreId));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Movie>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Movie?> GetByIdWithGenresAsync(int id)
        {
            return await _context.Movies
                .Include(x => x.MovieGenres)!
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AllGenresExistAsync(List<int> genreIds)
        {
            var count = await _context.Genres
                .CountAsync(x => genreIds.Contains(x.Id));

            return count == genreIds.Count;
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Movie>> GetMainFeatureMoviesAsync()
        {
            return await _context.Movies
                .Where(x => x.IsMainFeature)
                .ToListAsync();
        }
    }
}
