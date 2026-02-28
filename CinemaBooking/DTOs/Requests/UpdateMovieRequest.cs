namespace CinemaBooking.DTOs.Requests
{
    public class UpdateMovieRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int DurationMinutes { get; set; }
        public string? Status { get; set; }

        public IFormFile? PosterFile { get; set; }
        public IFormFile? TrailerFile { get; set; }

        public List<int>? GenreIds { get; set; }
    }
}
