using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Services.Interfaces;
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
    }
}
