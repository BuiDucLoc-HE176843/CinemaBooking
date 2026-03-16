namespace CinemaBooking.DTOs.Requests
{
    public class CreateBookingRequest
    {
        public int ShowtimeId { get; set; }

        public List<int> ShowtimeSeatIds { get; set; } = new();
    }
}
