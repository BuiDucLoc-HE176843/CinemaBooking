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
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        [HttpGet]
        public async Task<ApiResponse<PagedResult<ShowtimeResponse>>> GetShowtimes(
            [FromQuery] ShowtimeFilterRequest request)
        {
            var result = await _showtimeService.FilterAsync(request);

            return ApiResponse<PagedResult<ShowtimeResponse>>.Ok(result, "Lấy danh sách showtime thành công");
        }

        [HttpPost]
        public async Task<ApiResponse<object>> Create([FromBody] CreateShowtimeRequest request)
        {
            await _showtimeService.CreateAsync(request);

            return ApiResponse<object>.Ok("Tạo showtime thành công");
        }
    }
}
