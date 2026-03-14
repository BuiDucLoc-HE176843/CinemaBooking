namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class CreateShowtimeRequest
    {
        public int MovieId { get; set; }

        public int RoomId { get; set; }

        public DateTime StartTime { get; set; }
    }
}
