using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
