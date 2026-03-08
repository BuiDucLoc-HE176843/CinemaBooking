using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task AddRangeAsync(IEnumerable<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByRoomIdAsync(int roomId)
        {
            var seats = await _context.Seats
                .Where(x => x.RoomId == roomId)
                .ToListAsync();

            _context.Seats.RemoveRange(seats);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Seat>> GetNoDeleteByRoomIdAsync(int roomId)
        {
            return await _context.Seats.Where(x => x.IsDeleted == false)
                .Where(x => x.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<List<Seat>> GetAllByRoomIdAsync(int roomId)
        {
            return await _context.Seats
                .Where(x => x.RoomId == roomId)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.UpdateRange(seats);
            await _context.SaveChangesAsync();
        }
    }
}
