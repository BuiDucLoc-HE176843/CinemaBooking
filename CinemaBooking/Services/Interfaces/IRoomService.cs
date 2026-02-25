using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IRoomService
    {
        Task AddAsync(CreateRoomRequest request);
        Task UpdateAsync(int id, UpdateRoomRequest request);
        Task<PagedResult<RoomResponse>> GetByTheaterIdAsync(int theaterId, PaginationRequest request);
    }
}
