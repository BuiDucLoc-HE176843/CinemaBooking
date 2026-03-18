namespace CinemaBooking_RazorPage.DTOs.Responses
{
    public class BookingSeatResponse
    {
        public int ShowtimeSeatId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }
    }

    public class BookingResponse
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string PosterUrl { get; set; }
        public string TheaterName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string TransactionContent { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu chiếu
        public DateTime EndTime { get; set; }    // Thời gian kết thúc chiếu

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<BookingSeatResponse> Seats { get; set; } = new();
    }
}
