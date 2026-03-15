using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Implementations;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(
            IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(int userId, CreateReviewRequest request)
        {
            var movieExists = await _reviewRepository.ExistsMovieAsync(request.MovieId);

            if (!movieExists)
                throw new AppException("Phim không tồn tại");

            var alreadyReviewed = await _reviewRepository
                .ExistsAsync(userId, request.MovieId);

            if (alreadyReviewed)
                throw new AppException("Bạn đã đánh giá phim này rồi");

            var review = new Review
            {
                UserId = userId,
                MovieId = request.MovieId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }

        public async Task<List<ReviewResponse>> GetReviewsByMovieAsync(int movieId)
        {
            var movieExists = await _reviewRepository.ExistsMovieAsync(movieId);

            if (!movieExists)
                throw new AppException("Phim không tồn tại");

            return await _reviewRepository.GetByMovieIdAsync(movieId);
        }
    }
}
