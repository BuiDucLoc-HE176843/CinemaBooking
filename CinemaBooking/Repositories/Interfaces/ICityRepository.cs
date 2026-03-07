using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
    }
}
