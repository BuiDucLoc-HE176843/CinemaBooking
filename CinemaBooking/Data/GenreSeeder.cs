using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class GenreSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Genres.AnyAsync())
                return;

            var genres = new List<Genre>
            {
                new Genre { Name = "Hành động" },
                new Genre { Name = "Phiêu lưu" },
                new Genre { Name = "Hài" },
                new Genre { Name = "Kinh dị" },
                new Genre { Name = "Tình cảm" },
                new Genre { Name = "Hoạt hình" },
                new Genre { Name = "Khoa học viễn tưởng" },
                new Genre { Name = "Giả tưởng" },
                new Genre { Name = "Tâm lý" },
                new Genre { Name = "Tài liệu" },
                new Genre { Name = "Gia đình" },
                new Genre { Name = "Bí ẩn" }
            };

            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }
    }
}
