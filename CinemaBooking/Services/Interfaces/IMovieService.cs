using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<PagedResult<MovieResponse>> GetMoviesAsync(MovieFilterRequest request);
        Task UpdateMovieAsync(int id, UpdateMovieRequest request);
        Task AddMovieAsync(CreateMovieRequest request);
    }
}
