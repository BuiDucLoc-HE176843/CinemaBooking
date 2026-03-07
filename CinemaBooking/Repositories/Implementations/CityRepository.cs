using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
