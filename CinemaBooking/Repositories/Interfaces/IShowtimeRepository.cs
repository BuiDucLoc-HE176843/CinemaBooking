using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IShowtimeRepository
    {
        Task<Showtime?> GetByIdAsync(int id);

        Task<bool> MovieExistsAsync(int movieId);
        Task<bool> RoomExistsAsync(int roomId);

        Task<PagedResult<ShowtimeResponse>> FilterAsync(ShowtimeFilterRequest request);

        Task<Movie?> GetMovieAsync(int movieId);

        Task<Room?> GetRoomAsync(int roomId);

        Task<List<Seat>> GetSeatsByRoomAsync(int roomId);

        Task<List<Showtime>> GetShowtimesByRoomAsync(int roomId);

        Task AddShowtimeAsync(Showtime showtime);

        Task AddShowtimeSeatsAsync(List<ShowtimeSeat> showtimeSeats);
    }
}
