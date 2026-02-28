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
    public class TheatersController : ControllerBase
    {
        private readonly ITheaterService _service;

        public TheatersController(ITheaterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<PagedResult<TheaterResponse>>> GetAll([FromQuery] TheaterFilterRequest request)
        {
            var result = await _service.GetPagedAsync(request);

            return ApiResponse<PagedResult<TheaterResponse>>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<TheaterResponse>> Create([FromBody] CreateTheaterRequest request)
        {
            var result = await _service.CreateAsync(request);

            return ApiResponse<TheaterResponse>.Ok(result, "Tạo rạp thành công");
        }


        [HttpPut("{id}")]
        public async Task<ApiResponse<TheaterResponse>> Update(int id, [FromBody] UpdateTheaterRequest request)
        {
            var result = await _service.UpdateAsync(id, request);

            return ApiResponse<TheaterResponse>.Ok(result, "Cập nhật rạp thành công");
        }
    }
}
