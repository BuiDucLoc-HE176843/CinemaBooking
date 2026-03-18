using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> CreateAsync(Booking booking);
        Task UpdateAsync(Booking booking);

        Task<List<ShowtimeSeat>> GetByIdsAsync(List<int> ids);
        Task UpdateRangeAsync(List<ShowtimeSeat> seats);

        Task AddRangeAsync(List<BookingSeat> bookingSeats);

        Task<Booking?> GetByIdAsync(int id);
        IQueryable<Booking> GetByUserIdQueryable(int userId);
    }
}
