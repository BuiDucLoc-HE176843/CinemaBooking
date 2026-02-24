using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface ITheaterRepository
    {
        Task<PagedResult<Theater>> GetPagedAsync(TheaterFilterRequest request);
    }
}
