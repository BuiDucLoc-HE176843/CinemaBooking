using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;

        public ShowtimeService(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
        }

        public async Task<PagedResult<ShowtimeResponse>> FilterAsync(ShowtimeFilterRequest request)
        {
            if (request.Id.HasValue)
            {
                var showtime = await _showtimeRepository.GetByIdAsync(request.Id.Value);
                if (showtime == null)
                    throw new AppException("Showtime không tồn tại");
            }

            if (request.MovieId.HasValue)
            {
                var exists = await _showtimeRepository.MovieExistsAsync(request.MovieId.Value);
                if (!exists)
                    throw new AppException("Movie không tồn tại");
            }

            if (request.RoomId.HasValue)
            {
                var exists = await _showtimeRepository.RoomExistsAsync(request.RoomId.Value);
                if (!exists)
                    throw new AppException("Room không tồn tại");
            }

            return await _showtimeRepository.FilterAsync(request);
        }

        public async Task CreateAsync(CreateShowtimeRequest request)
        {
            var movie = await _showtimeRepository.GetMovieAsync(request.MovieId);

            if (movie == null)
                throw new AppException("Movie không tồn tại");

            var room = await _showtimeRepository.GetRoomAsync(request.RoomId);

            if (room == null)
                throw new AppException("Room không tồn tại");

            var startTime = request.StartTime;
            if (startTime < DateTime.Now)
                throw new AppException("Thời gian chiếu phải là tương lai");

            var endTime = startTime.AddMinutes(movie.DurationMinutes);

            // lấy showtime của room
            var showtimes = await _showtimeRepository.GetShowtimesByRoomAsync(request.RoomId);

            foreach (var st in showtimes)
            {
                var stStart = st.StartTime;
                var stEnd = st.EndTime;

                // check trùng giờ
                if (startTime < stEnd && endTime > stStart)
                {
                    throw new AppException("Thời gian chiếu bị trùng với showtime khác");
                }

                // check khoảng cách tối thiểu 30 phút
                var diff1 = Math.Abs((startTime - stEnd).TotalMinutes);
                var diff2 = Math.Abs((stStart - endTime).TotalMinutes);

                if (diff1 < 30 || diff2 < 30)
                {
                    throw new AppException("Showtime phải cách showtime khác ít nhất 30 phút");
                }
            }

            var showtime = new Showtime
            {
                MovieId = request.MovieId,
                RoomId = request.RoomId,
                StartTime = startTime,
                EndTime = endTime
            };

            await _showtimeRepository.AddShowtimeAsync(showtime);

            // lấy ghế của room
            var seats = await _showtimeRepository.GetSeatsByRoomAsync(request.RoomId);

            var showtimeSeats = seats.Select(seat => new ShowtimeSeat
            {
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                Status = SeatStatus.Available
            }).ToList();

            await _showtimeRepository.AddShowtimeSeatsAsync(showtimeSeats);
        }
    }
}
