using CinemaBooking.Configuration;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int id);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task<bool> ExistsAsync(int id);
        Task<PagedResult<Room>> GetByTheaterIdAsync(int theaterId, PaginationRequest request);
    }
}
