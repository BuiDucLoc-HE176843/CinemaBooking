using CinemaBooking.Models;

namespace CinemaBooking.Services.Interfaces
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllAsync();
        Task<List<Genre>> GetByMovieIdAsync(int movieId);
    }
}
