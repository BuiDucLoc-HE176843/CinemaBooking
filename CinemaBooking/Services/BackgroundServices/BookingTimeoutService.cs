using CinemaBooking.Data;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Services.BackgroundServices
{
    public class BookingTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingTimeoutService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // Kiểm tra mỗi phút một lần

        public BookingTimeoutService(IServiceProvider serviceProvider, ILogger<BookingTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CancelExpiredBookings();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy service tự động hủy đơn hàng hết hạn.");
                }

                // Đợi 1 phút trước khi quét đợt tiếp theo
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CancelExpiredBookings()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // 1. Xác định mốc thời gian hết hạn (Hiện tại - 5 phút)
                var expirationTime = DateTime.UtcNow.AddMinutes(-5);

                // 2. Tìm các Booking quá hạn mà vẫn đang ở trạng thái Pending
                // Tối ưu: Chỉ lấy những dữ liệu cần thiết kèm theo bảng Seat liên quan
                var expiredBookings = await context.Set<Booking>()
                    .Include(b => b.BookingSeats)
                        .ThenInclude(bs => bs.ShowtimeSeat)
                    .Where(b => b.Status == BookingStatus.Pending && b.BookingDate <= expirationTime)
                    .ToListAsync();

                if (!expiredBookings.Any()) return;

                _logger.LogInformation($"Phát hiện {expiredBookings.Count} đơn hàng quá hạn thanh toán. Đang xử lý hủy...");

                foreach (var booking in expiredBookings)
                {
                    // A. Chuyển trạng thái Booking sang Cancelled
                    booking.Status = BookingStatus.Cancelled;

                    // B. Giải phóng các ghế đã giữ chỗ
                    if (booking.BookingSeats != null)
                    {
                        foreach (var seatAssignment in booking.BookingSeats)
                        {
                            if (seatAssignment.ShowtimeSeat != null)
                            {
                                // Chỉ trả về Available nếu ghế đó đang bị giữ (Pending)
                                // Tránh trường hợp ghế đã được thanh toán bởi một luồng khác
                                if (seatAssignment.ShowtimeSeat.Status == SeatStatus.Pending)
                                {
                                    seatAssignment.ShowtimeSeat.Status = SeatStatus.Available;
                                }
                            }
                        }
                    }
                }

                // 3. Lưu toàn bộ thay đổi xuống DB trong một Transaction duy nhất
                await context.SaveChangesAsync();

                _logger.LogInformation($"Đã hủy thành công {expiredBookings.Count} đơn hàng quá hạn.");
            }
        }
    }
}
