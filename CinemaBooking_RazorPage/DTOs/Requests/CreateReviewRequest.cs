using System.ComponentModel.DataAnnotations;

namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class CreateReviewRequest
    {
        [Required]
        public int MovieId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
