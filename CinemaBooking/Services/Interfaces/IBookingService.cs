using CinemaBooking.DTOs.Requests;

namespace CinemaBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task CreateBookingAsync(int userId, CreateBookingRequest request);
    }
}
