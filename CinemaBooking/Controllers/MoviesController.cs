using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<ApiResponse<PagedResult<MovieResponse>>> GetMovies(
            [FromQuery] MovieFilterRequest request)
        {
            var result = await _movieService.GetMoviesAsync(request);

            return ApiResponse<PagedResult<MovieResponse>>
                .Ok(result, "Lấy danh sách phim thành công");
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<object>> UpdateMovie(int id, [FromForm] UpdateMovieRequest request)
        {
            await _movieService.UpdateMovieAsync(id, request);

            return ApiResponse<object>.Ok("Cập nhật phim thành công");
        }

        [HttpPost]
        public async Task<ApiResponse<object>> AddMovie([FromForm] CreateMovieRequest request)
        {
            await _movieService.AddMovieAsync(request);

            return ApiResponse<object>.Ok("Thêm phim thành công");
        }
    }
}
