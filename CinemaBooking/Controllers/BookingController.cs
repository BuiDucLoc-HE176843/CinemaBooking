using CinemaBooking.DTOs.Requests;
using CinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CinemaBooking.Configuration;

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
        public async Task<ApiResponse<object>> CreateBooking(
    [FromBody] CreateBookingRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _bookingService.CreateBookingAsync(userId, request);

            return ApiResponse<object>.Ok("Đặt vé thành công");
        }
    }
}
