using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IShowtimeSeatRepository
    {
        Task<List<ShowtimeSeat>> GetSeatsAsync(int? id, int? showtimeId);
    }
}
