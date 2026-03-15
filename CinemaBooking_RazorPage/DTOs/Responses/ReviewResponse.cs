namespace CinemaBooking_RazorPage.DTOs.Responses
{
    public class ReviewResponse
    {
        public string? UserFullName { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
