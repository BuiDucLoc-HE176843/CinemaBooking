using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<int> CreateBookingAsync(int userId, CreateBookingRequest request);
        Task<BookingResponse> GetByIdAsync(int id);
        Task<List<BookingResponse>> GetMyBookingsAsync(int userId);
    }
}
