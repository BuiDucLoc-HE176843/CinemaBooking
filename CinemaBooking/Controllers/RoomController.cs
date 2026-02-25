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
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<ApiResponse<object>> Add([FromBody] CreateRoomRequest request)
        {
            await _roomService.AddAsync(request);

            return ApiResponse<object>.Ok("Tạo phòng thành công");
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<object>> Update(int id, [FromBody] UpdateRoomRequest request)
        {
            await _roomService.UpdateAsync(id, request);

            return ApiResponse<object>.Ok("Cập nhật phòng thành công");
        }

        [HttpGet("{theaterId}")]
        public async Task<ApiResponse<PagedResult<RoomResponse>>> GetByTheater(int theaterId, [FromQuery] PaginationRequest request)
        {
            var result = await _roomService.GetByTheaterIdAsync(theaterId, request);

            return ApiResponse<PagedResult<RoomResponse>>.Ok(result);
        }
    }
}
