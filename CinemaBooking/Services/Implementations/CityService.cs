using CinemaBooking.DTOs.Responses;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CityResponse>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllAsync();

            return cities.Select(x => new CityResponse
            {
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}
