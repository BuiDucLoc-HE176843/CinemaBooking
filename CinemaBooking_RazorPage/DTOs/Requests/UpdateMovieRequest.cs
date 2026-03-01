namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class UpdateMovieRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Status { get; set; }

        public bool IsMainFeature { get; set; } = false;

        public IFormFile? PosterFile { get; set; }
        public IFormFile? TrailerFile { get; set; }

        public List<int>? GenreIds { get; set; }
    }
}
