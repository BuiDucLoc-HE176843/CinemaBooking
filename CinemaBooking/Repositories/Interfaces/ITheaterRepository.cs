using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface ITheaterRepository
    {
        Task<PagedResult<Theater>> GetPagedAsync(TheaterFilterRequest request);
        Task AddAsync(Theater theater);
        Task UpdateAsync(Theater theater);
        Task<Theater?> GetByIdAsync(int id);
        Task<bool> CityExistsAsync(int cityId);
    }
}
