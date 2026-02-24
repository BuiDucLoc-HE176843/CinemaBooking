using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface ITheaterService
    {
        Task<PagedResult<TheaterResponse>> GetPagedAsync(TheaterFilterRequest request);
    }
}
