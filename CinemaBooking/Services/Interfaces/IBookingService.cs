using CinemaBooking.DTOs.Requests;

namespace CinemaBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<int> CreateBookingAsync(int userId, CreateBookingRequest request);
    }
}
