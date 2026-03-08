using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<SeatResponse> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new AppException("Id không hợp lệ");

            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                throw new AppException("Ghế không tồn tại");

            return new SeatResponse
            {
                Id = seat.Id,
                RoomId = seat.RoomId,
                RowNumber = seat.RowNumber,
                ColumnNumber = seat.ColumnNumber,
                Type = seat.Type,
                Status = seat.Status
            };
        }

        public async Task<IEnumerable<SeatResponse>> GetByRoomIdAsync(int roomId)
        {
            if (roomId <= 0)
                throw new AppException("RoomId không hợp lệ");

            var seats = await _seatRepository.GetNoDeleteByRoomIdAsync(roomId);

            if (!seats.Any())
                throw new AppException("Phòng không có ghế nào");

            return seats.Select(x => new SeatResponse
            {
                Id = x.Id,
                RoomId = x.RoomId,
                RowNumber = x.RowNumber,
                ColumnNumber = x.ColumnNumber,
                Type = x.Type,
                Status = x.Status
            });
        }
    }
}
