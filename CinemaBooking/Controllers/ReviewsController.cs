using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CinemaBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResponse<object>> AddReview([FromBody] CreateReviewRequest request)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _reviewService.AddReviewAsync(userId, request);

            return ApiResponse<object>.Ok("Đánh giá phim thành công");
        }

        [HttpGet("movie/{movieId}")]
        public async Task<ApiResponse<List<ReviewResponse>>> GetReviewsByMovie(int movieId)
        {
            var result = await _reviewService.GetReviewsByMovieAsync(movieId);

            return ApiResponse<List<ReviewResponse>>
                .Ok(result, "Lấy danh sách review thành công");
        }
    }
}
