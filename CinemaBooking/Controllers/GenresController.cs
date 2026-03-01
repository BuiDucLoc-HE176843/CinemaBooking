using CinemaBooking.Models;
using CinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CinemaBooking.Configuration;

namespace CinemaBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET: api/genres
        [HttpGet]
        public async Task<ApiResponse<List<Genre>>> GetAll()
        {
            var result = await _genreService.GetAllAsync();
            return ApiResponse<List<Genre>>.Ok(result, "Lấy danh sách thể loại thành công");
        }

        // GET: api/genres/movie/5
        [HttpGet("movie/{movieId}")]
        public async Task<ApiResponse<List<Genre>>> GetByMovieId(int movieId)
        {
            var result = await _genreService.GetByMovieIdAsync(movieId);
            return ApiResponse<List<Genre>>.Ok(result, "Lấy thể loại theo phim thành công");
        }
    }
}
