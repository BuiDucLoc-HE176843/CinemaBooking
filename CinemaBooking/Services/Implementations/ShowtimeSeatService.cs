using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;
using CinemaBooking.Enums;

namespace CinemaBooking.Services.Implementations
{
    public class ShowtimeSeatService : IShowtimeSeatService
    {
        private readonly IShowtimeSeatRepository _repository;

        public ShowtimeSeatService(IShowtimeSeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ShowtimeSeatResponse>> GetSeatsAsync(int? id, int? showtimeId)
        {
            var seats = await _repository.GetSeatsAsync(id, showtimeId);

            return seats.Select(x =>
            {
                var seat = x.Seat!;
                var room = seat.Room!;

                var price = seat.Type == SeatType.Vip
                    ? room.VipPrice
                    : room.RegularPrice;

                return new ShowtimeSeatResponse
                {
                    Id = x.Id,
                    ShowtimeId = x.ShowtimeId,
                    SeatId = seat.Id,
                    RowNumber = seat.RowNumber,
                    ColumnNumber = seat.ColumnNumber,
                    SeatType = seat.Type.ToString(),
                    Status = x.Status.ToString(),
                    Price = price
                };
            }).ToList();
        }
    }
}
