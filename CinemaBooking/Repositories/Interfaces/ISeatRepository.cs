using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task AddRangeAsync(IEnumerable<Seat> seats);
        Task DeleteByRoomIdAsync(int roomId);
        Task<List<Seat>> GetAllByRoomIdAsync(int roomId);
        Task<List<Seat>> GetNoDeleteByRoomIdAsync(int roomId);
        Task UpdateRangeAsync(IEnumerable<Seat> seats);
        Task<Seat?> GetByIdAsync(int id);
    }
}
