using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShowtimeSeat>> GetByIdsAsync(List<int> ids)
        {
            return await _context.ShowtimeSeats
                .Include(x => x.Seat)
                    .ThenInclude(s => s.Room)
                .Include(x => x.BookingSeats)
                    .ThenInclude(bs => bs.Booking)
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(List<ShowtimeSeat> seats)
        {
            _context.ShowtimeSeats.UpdateRange(seats);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<BookingSeat> bookingSeats)
        {
            _context.BookingSeats.AddRange(bookingSeats);
            await _context.SaveChangesAsync();
        }
    }
}
