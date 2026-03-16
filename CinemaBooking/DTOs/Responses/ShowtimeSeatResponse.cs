namespace CinemaBooking.DTOs.Responses
{
    public class ShowtimeSeatResponse
    {
        public int Id { get; set; }

        public int ShowtimeId { get; set; }

        public int SeatId { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string SeatType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
