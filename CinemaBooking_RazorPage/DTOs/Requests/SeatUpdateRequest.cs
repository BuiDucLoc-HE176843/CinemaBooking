namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class SeatUpdateRequest
    {
        public SeatType Type { get; set; }

        public bool IsDisabled { get; set; }
    }

    public enum SeatType
    {
        Regular,
        Vip
    }
}
