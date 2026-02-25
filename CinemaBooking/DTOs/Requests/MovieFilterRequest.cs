using CinemaBooking.Configuration;

namespace CinemaBooking.DTOs.Requests
{
    public class MovieFilterRequest : PaginationRequest
    {
        public string? Title { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? Status { get; set; }   // truyền string để validate

        public int? GenreId { get; set; }
    }
}
