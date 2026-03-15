using CinemaBooking.Data;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int userId, int movieId)
        {
            return await _context.Reviews
                .AnyAsync(x => x.UserId == userId && x.MovieId == movieId);
        }

        public async Task<List<ReviewResponse>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Reviews
                .Where(x => x.MovieId == movieId)
                .Select(x => new ReviewResponse
                {
                    UserFullName = x.User!.FullName,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsMovieAsync(int movieId)
        {
            return await _context.Movies.AnyAsync(x => x.Id == movieId);
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
    }
}
