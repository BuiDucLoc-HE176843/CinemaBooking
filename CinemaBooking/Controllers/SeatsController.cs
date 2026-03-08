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
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<SeatResponse>> GetById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            return ApiResponse<SeatResponse>.Ok(seat, "Lấy ghế thành công");
        }

        [HttpGet("room/{roomId}")]
        public async Task<ApiResponse<IEnumerable<SeatResponse>>> GetByRoomId(int roomId)
        {
            var seats = await _seatService.GetByRoomIdAsync(roomId);

            return ApiResponse<IEnumerable<SeatResponse>>
                .Ok(seats, "Lấy danh sách ghế thành công");
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<object>> Update(int id, [FromBody] SeatUpdateRequest request)
        {
            await _seatService.UpdateAsync(id, request);

            return ApiResponse<object>.Ok("Cập nhật ghế thành công");
        }
    }
}
