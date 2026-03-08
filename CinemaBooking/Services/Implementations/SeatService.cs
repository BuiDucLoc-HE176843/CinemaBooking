using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
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
                IsDisabled = seat.IsDisabled
            };
        }

        public async Task<IEnumerable<SeatResponse>> GetByRoomIdAsync(int roomId)
        {
            var roomExists = await _seatRepository.RoomExistsAsync(roomId);

            if (!roomExists)
                throw new AppException("Phòng không tồn tại");

            var seats = await _seatRepository.GetNoDeleteByRoomIdAsync(roomId);

            if (!seats.Any())
                throw new AppException("Phòng chưa có ghế");

            return seats.Select(x => new SeatResponse
            {
                Id = x.Id,
                RoomId = x.RoomId,
                RowNumber = x.RowNumber,
                ColumnNumber = x.ColumnNumber,
                Type = x.Type,
                IsDisabled = x.IsDisabled
            });
        }

        public async Task UpdateAsync(int id, SeatUpdateRequest request)
        {
            var seat = await _seatRepository.GetByIdAsync(id);

            if (seat == null)
                throw new AppException("Ghế không tồn tại");

            // update fields
            seat.Type = request.Type;
            seat.IsDisabled = request.IsDisabled;
            seat.UpdatedAt = DateTime.UtcNow;

            await _seatRepository.UpdateAsync(seat);
        }
    }
}
