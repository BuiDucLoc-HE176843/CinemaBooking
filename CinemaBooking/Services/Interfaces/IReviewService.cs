using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;

namespace CinemaBooking.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(int userId, CreateReviewRequest request);
        Task<List<ReviewResponse>> GetReviewsByMovieAsync(int movieId);
    }
}
