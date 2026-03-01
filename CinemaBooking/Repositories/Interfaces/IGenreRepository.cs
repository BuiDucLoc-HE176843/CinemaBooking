using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync();
        Task<List<Genre>> GetByMovieIdAsync(int movieId);
    }
}
