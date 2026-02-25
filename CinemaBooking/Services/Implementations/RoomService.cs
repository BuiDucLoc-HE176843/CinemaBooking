using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Implementations;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task AddAsync(CreateRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new AppException("Tên phòng không được để trống");

            if (!await _roomRepository.ExistsAsync(request.TheaterId))
                throw new AppException("Rạp không tồn tại");

            if(request.Rows <= 0 || request.Columns <=0)
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
                throw new AppException("Số lượng ghế không hợp lệ");

            // Chỉ update field có truyền vào
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
