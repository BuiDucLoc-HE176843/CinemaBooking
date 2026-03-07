using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface ICityService
    {
        Task<IEnumerable<CityResponse>> GetAllAsync();
    }
}
