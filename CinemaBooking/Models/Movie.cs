using CinemaBooking.Enums;
using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int DurationMinutes { get; set; }

        [MaxLength(255)]
        public string? TrailerUrl { get; set; }

        [MaxLength(255)]
        public string? PosterUrl { get; set; }

        [Required]
        public MovieStatus Status { get; set; } = MovieStatus.Upcoming;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsMainFeature { get; set; } = false;

        // Navigation
        public ICollection<Showtime>? Showtimes { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        // Many-to-many với Genre
        public ICollection<MovieGenre>? MovieGenres { get; set; }
    }
}
