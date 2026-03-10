using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;

namespace CinemaBooking.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<PagedResult<ShowtimeResponse>> FilterAsync(ShowtimeFilterRequest request);
        Task CreateAsync(CreateShowtimeRequest request);
    }
}
