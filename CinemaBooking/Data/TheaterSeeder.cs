using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class TheaterSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Theaters.AnyAsync())
                return;

            var cities = await context.Cities.ToListAsync();

            var haNoi = cities.First(c => c.Name == "Hà Nội").Id;
            var hcm = cities.First(c => c.Name == "TP. Hồ Chí Minh").Id;
            var daNang = cities.First(c => c.Name == "Đà Nẵng").Id;

            var theaters = new List<Theater>
            {
                new Theater
                {
                    Name = "CGV Vincom Bà Triệu",
                    Address = "191 Bà Triệu, Hai Bà Trưng",
                    CityId = haNoi,
                    CreatedAt = DateTime.UtcNow
                },
                new Theater
                {
                    Name = "Lotte Cinema West Lake",
                    Address = "Lotte Mall Tây Hồ",
                    CityId = haNoi,
                    CreatedAt = DateTime.UtcNow
                },
                new Theater
                {
                    Name = "CGV Landmark 81",
                    Address = "Vinhomes Central Park, Bình Thạnh",
                    CityId = hcm,
                    CreatedAt = DateTime.UtcNow
                },
                new Theater
                {
                    Name = "Galaxy Nguyễn Du",
                    Address = "116 Nguyễn Du, Quận 1",
                    CityId = hcm,
                    CreatedAt = DateTime.UtcNow
                },
                new Theater
                {
                    Name = "Lotte Cinema Đà Nẵng",
                    Address = "Lotte Mart, Hải Châu",
                    CityId = daNang,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Theaters.AddRangeAsync(theaters);
            await context.SaveChangesAsync();
        }
    }
}
