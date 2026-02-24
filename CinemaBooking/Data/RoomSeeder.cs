using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class RoomSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Rooms.AnyAsync())
                return;

            var theaters = await context.Theaters.ToListAsync();

            var rooms = new List<Room>();

            foreach (var theater in theaters)
            {
                rooms.Add(new Room
                {
                    TheaterId = theater.Id,
                    Name = "Phòng 1",
                    Rows = 8,
                    Columns = 8,
                    RegularPrice = 90000,
                    VipPrice = 150000,
                    CreatedAt = DateTime.UtcNow
                });

                rooms.Add(new Room
                {
                    TheaterId = theater.Id,
                    Name = "Phòng 2",
                    Rows = 10,
                    Columns = 10,
                    RegularPrice = 100000,
                    VipPrice = 170000,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.Rooms.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }
    }
}
