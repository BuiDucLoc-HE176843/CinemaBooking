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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResponse<object>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bookingId = await _bookingService.CreateBookingAsync(userId, request);

            return ApiResponse<object>.Ok(new
            {
                BookingId = bookingId
            }, "Đặt vé thành công");
        }

        // 🔹 GET booking theo id
        [HttpGet("{id}")]
        public async Task<ApiResponse<BookingResponse>> GetById(int id)
        {
            var result = await _bookingService.GetByIdAsync(id);

            return ApiResponse<BookingResponse>.Ok(result);
        }

        // 🔹 GET booking của user hiện tại
        [Authorize]
        [HttpGet("me")]
        public async Task<ApiResponse<PagedResult<BookingResponse>>> GetMyBookings([FromQuery] PaginationRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _bookingService.GetMyBookingsAsync(userId, request);

            return ApiResponse<PagedResult<BookingResponse>>.Ok(result);
        }
    }
}
