using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<PagedResult<MovieResponse>> GetMoviesAsync(MovieFilterRequest request);
    }
}
