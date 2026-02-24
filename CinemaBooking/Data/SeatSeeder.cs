using CinemaBooking.Enums;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class SeatSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Seats.AnyAsync())
                return;

            var rooms = await context.Rooms.ToListAsync();
            var seats = new List<Seat>();

            foreach (var room in rooms)
            {
                for (int r = 1; r <= room.Rows; r++)
                {
                    for (int c = 1; c <= room.Columns; c++)
                    {
                        var seatType = r >= room.Rows - 1
                            ? SeatType.Vip
                            : SeatType.Regular;

                        seats.Add(new Seat
                        {
                            RoomId = room.Id,
                            RowNumber = r,
                            ColumnNumber = c,
                            Type = seatType,
                            Status = SeatStatus.Available,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await context.Seats.AddRangeAsync(seats);
            await context.SaveChangesAsync();
        }
    }
}
