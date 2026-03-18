using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(
            IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<int> CreateBookingAsync(int userId, CreateBookingRequest request)
        {
            if (request.ShowtimeSeatIds == null || !request.ShowtimeSeatIds.Any())
                throw new AppException("Danh sách ghế không hợp lệ");

            // lấy danh sách ghế
            var showtimeSeats = await _bookingRepository.GetByIdsAsync(request.ShowtimeSeatIds);

            if (showtimeSeats.Count != request.ShowtimeSeatIds.Count)
                throw new AppException("Một số ghế không tồn tại");

            // kiểm tra ghế đã có booking chưa
            foreach (var seat in showtimeSeats)
            {
                var hasActiveBooking = seat.BookingSeats?
                    .Any(bs => bs.Booking!.Status == BookingStatus.Pending
                            || bs.Booking.Status == BookingStatus.Paid) ?? false;

                if (hasActiveBooking)
                    throw new AppException("Ghế đã được đặt");
            }

            // random transaction code 8 ký tự chữ + số
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var transactionContent = new string(
                Enumerable.Range(0, 8)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());

            // 1️⃣ tạo booking trước
            var booking = new Booking
            {
                UserId = userId,
                ShowtimeId = request.ShowtimeId,
                Status = BookingStatus.Pending,
                TotalPrice = 0,
                TransactionContent = transactionContent
            };

            booking = await _bookingRepository.CreateAsync(booking);

            decimal totalPrice = 0;

            var bookingSeats = new List<BookingSeat>();

            // 2️⃣ tạo booking seat
            foreach (var showtimeSeat in showtimeSeats)
            {
                var room = showtimeSeat.Seat!.Room!;

                decimal price = showtimeSeat.Seat.Type == SeatType.Vip
                    ? room.VipPrice
                    : room.RegularPrice;

                totalPrice += price;

                bookingSeats.Add(new BookingSeat
                {
                    BookingId = booking.Id,
                    ShowtimeSeatId = showtimeSeat.Id,
                    PriceAtBooking = price
                });

                // update trạng thái ghế
                showtimeSeat.Status = SeatStatus.Pending;
            }

            await _bookingRepository.AddRangeAsync(bookingSeats);

            // 3️⃣ update trạng thái ghế
            await _bookingRepository.UpdateRangeAsync(showtimeSeats);

            // 4️⃣ update lại booking
            booking.TotalPrice = totalPrice;

            await _bookingRepository.UpdateAsync(booking);

            return booking.Id;
        }

        public async Task<BookingResponse> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                throw new AppException("Booking không tồn tại");

            return MapToResponse(booking);
        }

        public async Task<List<BookingResponse>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);

            return bookings.Select(MapToResponse).ToList();
        }

        private BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                MovieName = booking.Showtime?.Movie?.Title ?? "",
                PosterUrl = booking.Showtime?.Movie?.PosterUrl ?? "",
                TheaterName = booking.Showtime?.Room?.Theater?.Name ?? "",
                RoomName = booking.Showtime?.Room?.Name ?? "",
                TransactionContent = booking.TransactionContent,
                BookingDate = booking.BookingDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status.ToString(),

                Seats = booking.BookingSeats?
                    .Select(s => new BookingSeatResponse
                    {
                        ShowtimeSeatId = s.ShowtimeSeatId,
                        SeatId = s.ShowtimeSeat?.Seat?.Id ?? 0,
                        Price = s.PriceAtBooking
                    }).ToList() ?? new List<BookingSeatResponse>()
            };
        }
    }
}
