using CinemaBooking.Configuration;
using CinemaBooking.Data;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly ApplicationDbContext _context;

        public TheaterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Theater>> GetPagedAsync(TheaterFilterRequest request)
        {
            var query = _context.Theaters
                .Include(x => x.City)
                .AsQueryable();

            // Filter Name
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(x => x.Name.Contains(request.Name));
            }

            // Filter Address
            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                query = query.Where(x => x.Address!.Contains(request.Address));
            }

            // Filter CityId
            if (request.CityId.HasValue)
            {
                query = query.Where(x => x.CityId == request.CityId.Value);
            }

            // Filter CityName
            if (!string.IsNullOrWhiteSpace(request.CityName))
            {
                query = query.Where(x => x.City!.Name.Contains(request.CityName));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<Theater>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}
