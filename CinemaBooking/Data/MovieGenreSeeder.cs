using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class MovieGenreSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.MovieGenres.AnyAsync())
                return;

            var movies = await context.Movies.ToListAsync();
            var genres = await context.Genres.ToListAsync();

            var rand = new Random();
            var movieGenres = new List<MovieGenre>();

            foreach (var movie in movies)
            {
                var selectedGenres = genres
                    .OrderBy(x => rand.Next())
                    .Take(rand.Next(1, 3)) // 1-2 thể loại
                    .ToList();

                foreach (var genre in selectedGenres)
                {
                    movieGenres.Add(new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genre.Id
                    });
                }
            }

            await context.MovieGenres.AddRangeAsync(movieGenres);
            await context.SaveChangesAsync();
        }
    }
}
