using CinemaBooking.DTOs.Responses;
using CinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CinemaBooking.Configuration;

namespace CinemaBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeSeatsController : ControllerBase
    {
        private readonly IShowtimeSeatService _service;

        public ShowtimeSeatsController(IShowtimeSeatService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<List<ShowtimeSeatResponse>>> GetSeats(
            [FromQuery] int? id,
            [FromQuery] int? showtimeId)
        {
            var result = await _service.GetSeatsAsync(id, showtimeId);

            return ApiResponse<List<ShowtimeSeatResponse>>
                .Ok(result, "Lấy danh sách ghế thành công");
        }
    }
}
