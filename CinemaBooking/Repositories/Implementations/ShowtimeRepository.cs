using CinemaBooking.Configuration;
using CinemaBooking.Data;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Repositories.Implementations
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetMovieAsync(int movieId)
        {
            return await _context.Movies
                .FirstOrDefaultAsync(x => x.Id == movieId && !x.IsDeleted);
        }

        public async Task<Room?> GetRoomAsync(int roomId)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(x => x.Id == roomId && !x.IsDeleted);
        }

        public async Task<List<Seat>> GetSeatsByRoomAsync(int roomId)
        {
            return await _context.Seats
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetShowtimesByRoomAsync(int roomId)
        {
            return await _context.Showtimes
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task AddShowtimeAsync(Showtime showtime)
        {
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task AddShowtimeSeatsAsync(List<ShowtimeSeat> showtimeSeats)
        {
            await _context.ShowtimeSeats.AddRangeAsync(showtimeSeats);
            await _context.SaveChangesAsync();
        }

        public async Task<Showtime?> GetByIdAsync(int id)
        {
            return await _context.Showtimes
                .Include(x => x.Movie)
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> MovieExistsAsync(int movieId)
        {
            return await _context.Movies.AnyAsync(x => x.Id == movieId);
        }

        public async Task<bool> RoomExistsAsync(int roomId)
        {
            return await _context.Rooms.AnyAsync(x => x.Id == roomId);
        }

        public async Task<PagedResult<ShowtimeResponse>> FilterAsync(ShowtimeFilterRequest request)
        {
            var query = _context.Showtimes
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.Id.HasValue)
                query = query.Where(x => x.Id == request.Id.Value);

            if (request.MovieId.HasValue)
                query = query.Where(x => x.MovieId == request.MovieId.Value);

            if (request.RoomId.HasValue)
                query = query.Where(x => x.RoomId == request.RoomId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.StartTime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ShowtimeResponse
                {
                    Id = x.Id,
                    MovieId = x.MovieId,
                    MovieTitle = x.Movie != null ? x.Movie.Title : null,

                    RoomId = x.RoomId,
                    RoomName = x.Room != null
                        ? x.Room.Name + " - Rạp: " + x.Room.Theater!.Name
                        : null,

                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                })
                .ToListAsync();

            return new PagedResult<ShowtimeResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}
