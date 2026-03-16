using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IShowtimeSeatService
    {
        Task<List<ShowtimeSeatResponse>> GetSeatsAsync(int? id, int? showtimeId);
    }
}
