using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class ShowtimeSeatRepository : IShowtimeSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShowtimeSeat>> GetSeatsAsync(int? id, int? showtimeId)
        {
            var query = _context.ShowtimeSeats
                .Include(x => x.Seat)
                    .ThenInclude(s => s!.Room)
                .Where(x =>
                    !x.IsDeleted &&
                    x.Seat != null &&
                    //!x.Seat.IsDeleted &&
                    !x.Seat.IsDisabled
                )
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (showtimeId.HasValue)
                query = query.Where(x => x.ShowtimeId == showtimeId.Value);

            return await query.ToListAsync();
        }
    }
}
