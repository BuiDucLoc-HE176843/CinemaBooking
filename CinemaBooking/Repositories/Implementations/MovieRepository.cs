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

        public async Task<PagedResult<Movie>> GetPagedAsync(
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

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(x => x.Title.Contains(title));

            if (releaseDate.HasValue)
                query = query.Where(x => x.ReleaseDate == releaseDate);

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
    }
}
