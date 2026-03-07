using CinemaBooking.DTOs.Responses;
using CinemaBooking.Services.Interfaces;
using CinemaBooking.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<CityResponse>>> GetAll()
        {
            var cities = await _cityService.GetAllAsync();

            return ApiResponse<IEnumerable<CityResponse>>
                .Ok(cities, "Lấy danh sách thành phố thành công");
        }
    }
}
