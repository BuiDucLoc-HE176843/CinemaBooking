using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class TheaterService : ITheaterService
    {
        private readonly ITheaterRepository _repository;

        public TheaterService(ITheaterRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<TheaterResponse>> GetPagedAsync(TheaterFilterRequest request)
        {
            var result = await _repository.GetPagedAsync(request);

            var response = new PagedResult<TheaterResponse>
            {
                Items = result.Items.Select(x => new TheaterResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    CityId = x.CityId,
                    CityName = x.City?.Name
                }),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return response;
        }
    }
}
