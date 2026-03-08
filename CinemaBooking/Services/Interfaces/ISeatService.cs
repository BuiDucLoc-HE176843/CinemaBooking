using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponse> GetByIdAsync(int id);

        Task<IEnumerable<SeatResponse>> GetByRoomIdAsync(int roomId);
    }
}
