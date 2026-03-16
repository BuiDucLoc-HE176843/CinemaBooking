namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class CreateBookingRequest
    {
        public int ShowtimeId { get; set; }

        public List<int> ShowtimeSeatIds { get; set; } = new();
    }
}
