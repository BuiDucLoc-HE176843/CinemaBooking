using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Implementations;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        private readonly ISeatRepository _seatRepository;

        public RoomService(IRoomRepository roomRepository, ISeatRepository seatRepository)
        {
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
        }

        public async Task<RoomResponse> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
                throw new AppException("Phòng không tồn tại");

            return new RoomResponse
            {
                Id = room.Id,
                TheaterId = room.TheaterId,
                Name = room.Name,
                Rows = room.Rows,
                Columns = room.Columns,
                RegularPrice = room.RegularPrice,
                VipPrice = room.VipPrice
            };
        }

        public async Task AddAsync(CreateRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new AppException("Tên phòng không được để trống");

            if (!await _roomRepository.ExistsAsync(request.TheaterId))
                throw new AppException("Rạp không tồn tại");

            if (request.Rows <= 0 || request.Columns <= 0)
                throw new AppException("Số lượng ghế không hợp lệ");

            if (request.RegularPrice <= 0 || request.VipPrice <= 0)
                throw new AppException("Giá tiền không hợp lệ");

            var room = new Room
            {
                TheaterId = request.TheaterId,
                Name = request.Name,
                Rows = request.Rows ?? 8,
                Columns = request.Columns ?? 8,
                RegularPrice = request.RegularPrice ?? 90000,
                VipPrice = request.VipPrice ?? 150000,
                CreatedAt = DateTime.UtcNow
            };

            await _roomRepository.AddAsync(room);

            // tạo ghế
            var seats = new List<Seat>();

            for (int r = 1; r <= room.Rows; r++)
            {
                for (int c = 1; c <= room.Columns; c++)
                {
                    seats.Add(new Seat
                    {
                        RoomId = room.Id,
                        RowNumber = r,
                        ColumnNumber = c,
                        Type = SeatType.Regular,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _seatRepository.AddRangeAsync(seats);
        }

        public async Task UpdateAsync(int id, UpdateRoomRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
                throw new AppException("Phòng không tồn tại");

            if (!await _roomRepository.ExistsAsync(request.TheaterId ?? room.TheaterId))
                throw new AppException("Rạp không tồn tại");

            if ((request.Rows.HasValue && request.Rows <= 0) || (request.Columns.HasValue && request.Columns <= 0))
                throw new AppException("Số lượng ghế không hợp lệ");

            if ((request.RegularPrice.HasValue && request.RegularPrice <= 0)
                || (request.VipPrice.HasValue && request.VipPrice <= 0))
                throw new AppException("Giá tiền không hợp lệ");

            bool seatLayoutChanged = false;

            if (request.Rows.HasValue && request.Rows.Value != room.Rows)
                seatLayoutChanged = true;

            if (request.Columns.HasValue && request.Columns.Value != room.Columns)
                seatLayoutChanged = true;

            if (request.TheaterId.HasValue)
                room.TheaterId = request.TheaterId.Value;

            if (!string.IsNullOrWhiteSpace(request.Name))
                room.Name = request.Name;

            if (request.Rows.HasValue)
                room.Rows = request.Rows.Value;

            if (request.Columns.HasValue)
                room.Columns = request.Columns.Value;

            if (request.RegularPrice.HasValue)
                room.RegularPrice = request.RegularPrice.Value;

            if (request.VipPrice.HasValue)
                room.VipPrice = request.VipPrice.Value;

            room.UpdatedAt = DateTime.UtcNow;

            await _roomRepository.UpdateAsync(room);

            if (seatLayoutChanged)
            {
                var seats = await _seatRepository.GetAllByRoomIdAsync(room.Id);

                int newRows = room.Rows;
                int newColumns = room.Columns;

                var newSeats = new List<Seat>();

                for (int r = 1; r <= newRows; r++)
                {
                    for (int c = 1; c <= newColumns; c++)
                    {
                        var existingSeat = seats
                            .FirstOrDefault(x => x.RowNumber == r && x.ColumnNumber == c);

                        if (existingSeat == null)
                        {
                            // Tạo ghế mới nếu trước đó chưa từng tồn tại
                            newSeats.Add(new Seat
                            {
                                RoomId = room.Id,
                                RowNumber = r,
                                ColumnNumber = c,
                                Type = SeatType.Regular,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // Nếu ghế trước đó bị soft delete → restore
                            if (existingSeat.IsDeleted)
                            {
                                existingSeat.IsDeleted = false;
                                existingSeat.UpdatedAt = DateTime.UtcNow;
                            }
                        }
                    }
                }

                // Soft delete ghế vượt layout
                foreach (var seat in seats)
                {
                    if (seat.RowNumber > newRows || seat.ColumnNumber > newColumns)
                    {
                        if (!seat.IsDeleted)
                        {
                            seat.IsDeleted = true;
                            seat.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                if (newSeats.Any())
                    await _seatRepository.AddRangeAsync(newSeats);

                await _seatRepository.UpdateRangeAsync(seats);
            }
        }

        public async Task<PagedResult<RoomResponse>> GetByTheaterIdAsync(int theaterId, PaginationRequest request)
        {
            // check theater tồn tại
            if (!await _roomRepository.ExistsAsync(theaterId))
                throw new AppException("Rạp không tồn tại");

            var result = await _roomRepository.GetByTheaterIdAsync(theaterId, request);

            var mappedItems = result.Items.Select(x => new RoomResponse
            {
                Id = x.Id,
                TheaterId = x.TheaterId,
                Name = x.Name,
                Rows = x.Rows,
                Columns = x.Columns,
                RegularPrice = x.RegularPrice,
                VipPrice = x.VipPrice
            });

            return new PagedResult<RoomResponse>
            {
                Items = mappedItems,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };
        }
    }
}
