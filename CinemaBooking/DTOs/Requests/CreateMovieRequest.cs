using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.DTOs.Requests
{
    public class CreateMovieRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        public string? Status { get; set; }

        public bool IsMainFeature { get; set; } = false;

        public IFormFile? PosterFile { get; set; }

        public IFormFile? TrailerFile { get; set; }

        public List<int>? GenreIds { get; set; }
    }
}
