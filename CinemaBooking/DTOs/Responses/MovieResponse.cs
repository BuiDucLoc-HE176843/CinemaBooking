using CinemaBooking.Enums;

namespace CinemaBooking.DTOs.Responses
{
    public class MovieResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int DurationMinutes { get; set; }

        public MovieStatus Status { get; set; }

        public string? PosterUrl { get; set; }

        public string? TrailerUrl { get; set; }

        public List<string> Genres { get; set; } = new();
    }
}
