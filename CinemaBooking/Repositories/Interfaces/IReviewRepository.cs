using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> ExistsAsync(int userId, int movieId);
        Task<bool> ExistsMovieAsync(int movieId);
        Task<List<ReviewResponse>> GetByMovieIdAsync(int movieId);
        Task AddAsync(Review review);
    }
}
