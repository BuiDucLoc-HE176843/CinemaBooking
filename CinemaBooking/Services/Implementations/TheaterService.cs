using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
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

        public async Task<TheaterResponse> CreateAsync(CreateTheaterRequest request)
        {
            if (!await _repository.CityExistsAsync(request.CityId))
                throw new AppException("City không tồn tại");

            var theater = new Theater
            {
                Name = request.Name,
                Address = request.Address,
                CityId = request.CityId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(theater);

            var created = await _repository.GetByIdAsync(theater.Id);

            return new TheaterResponse
            {
                Id = created!.Id,
                Name = created.Name,
                Address = created.Address,
                CityId = created.CityId,
                CityName = created.City?.Name
            };
        }

        public async Task<TheaterResponse> UpdateAsync(int id, UpdateTheaterRequest request)
        {
            var theater = await _repository.GetByIdAsync(id);

            if (theater == null)
                throw new AppException("Rạp không tồn tại");

            if (!await _repository.CityExistsAsync(request.CityId))
                throw new AppException("City không tồn tại");

            theater.Name = request.Name;
            theater.Address = request.Address;
            theater.CityId = request.CityId;
            theater.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(theater);

            return new TheaterResponse
            {
                Id = theater.Id,
                Name = theater.Name,
                Address = theater.Address,
                CityId = theater.CityId,
                CityName = theater.City?.Name
            };
        }
    }
}
