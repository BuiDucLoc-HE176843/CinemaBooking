using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class CitySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Cities.AnyAsync())
                return;

            var cities = new List<City>
            {
                new City { Name = "Hà Nội" },
                new City { Name = "TP. Hồ Chí Minh" },
                new City { Name = "Đà Nẵng" },
                new City { Name = "Hải Phòng" },
                new City { Name = "Cần Thơ" }
            };

            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();
        }
    }
}
