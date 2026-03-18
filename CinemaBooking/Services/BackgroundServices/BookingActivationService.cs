using CinemaBooking.Data;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Services.BackgroundServices
{
    public class BookingActivationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingActivationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // Kiểm tra mỗi 30 giây

        public BookingActivationService(IServiceProvider serviceProvider, ILogger<BookingActivationService> logger)
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
                    await ProcessPendingBookings();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xảy ra trong quá trình đối soát giao dịch.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingBookings()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Chỉ quét các booking trong 24h qua để thu hẹp vùng dữ liệu (Index scan)
                var timeLimit = DateTime.UtcNow.AddMinutes(-5);

                // BƯỚC 1: Sử dụng LINQ join với SQL để tìm các cặp khớp nhau ngay tại Database
                // Cách này cực nhanh vì SQL Server thực hiện Hash Join hoặc Merge Join
                var matchedData = await (from b in context.Set<Booking>()
                                         join t in context.Set<BankTransaction>()
                                         on b.TotalPrice equals t.AmountIn
                                         where b.Status == BookingStatus.Pending
                                               && b.BookingDate >= timeLimit
                                               && t.TransactionDate >= timeLimit
                                               // Lọc chuỗi ngay tại SQL
                                               && t.TransactionContent == b.TransactionContent
                                         select new
                                         {
                                             BookingId = b.Id,
                                             TransactionId = t.Id
                                         })
                                         .AsNoTracking()
                                         .ToListAsync();

                if (!matchedData.Any()) return;

                // BƯỚC 2: Lấy danh sách ID cần cập nhật
                var bookingIdsToUpdate = matchedData.Select(x => x.BookingId).Distinct().ToList();

                // BƯỚC 3: Cập nhật đồng loạt (Cần Include để xử lý logic Seat)
                // Lưu ý: Nếu dùng EF Core 7.0+ bạn có thể dùng ExecuteUpdate để nhanh hơn nữa
                var bookingsToUpdate = await context.Set<Booking>()
                    .Include(b => b.BookingSeats)
                        .ThenInclude(bs => bs.ShowtimeSeat)
                    .Where(b => bookingIdsToUpdate.Contains(b.Id))
                    .ToListAsync();

                foreach (var booking in bookingsToUpdate)
                {
                    booking.Status = BookingStatus.Paid;

                    if (booking.BookingSeats != null)
                    {
                        foreach (var bs in booking.BookingSeats)
                        {
                            if (bs.ShowtimeSeat != null && bs.ShowtimeSeat.Status == SeatStatus.Pending)
                            {
                                bs.ShowtimeSeat.Status = SeatStatus.Booked;
                            }
                        }
                    }
                }

                // BƯỚC 4: Lưu tất cả trong 1 Transaction
                await context.SaveChangesAsync();

                _logger.LogInformation($"Đã xử lý thành công {bookingsToUpdate.Count} giao dịch.");
            }
        }
    }
}
