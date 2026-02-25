using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface ITheaterService
    {
        Task<PagedResult<TheaterResponse>> GetPagedAsync(TheaterFilterRequest request);
        Task<TheaterResponse> CreateAsync(CreateTheaterRequest request);
        Task<TheaterResponse> UpdateAsync(int id, UpdateTheaterRequest request);
    }
}
