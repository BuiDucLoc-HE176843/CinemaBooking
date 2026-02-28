using CinemaBooking.Enums;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class MovieSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Movies.AnyAsync())
                return;

            var movies = new List<Movie>();

            for (int i = 1; i <= 16; i++)
            {
                movies.Add(new Movie
                {
                    Title = $"Phim mẫu số {i}",
                    Description = $"Đây là mô tả cho phim số {i}. Nội dung hấp dẫn, kịch tính và đáng xem.",
                    ReleaseDate = DateTime.UtcNow.AddDays(-i * 10),
                    DurationMinutes = 90 + (i % 4) * 15,
                    PosterUrl = $"/Poster/poster{i}.jpg",
                    TrailerUrl = $"/Trailer/Trailer{i}.mp4",
                    Status = (i % 2 == 0)
                                ? MovieStatus.Upcoming
                                : MovieStatus.Ongoing,
                    IsMainFeature = (i == 1),
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }
    }
}
