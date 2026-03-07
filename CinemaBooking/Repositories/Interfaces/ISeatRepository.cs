using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task AddRangeAsync(IEnumerable<Seat> seats);
        Task DeleteByRoomIdAsync(int roomId);
        Task<List<Seat>> GetByRoomIdAsync(int roomId);
        Task UpdateRangeAsync(IEnumerable<Seat> seats);
    }
}
