namespace CinemaBooking.DTOs.Responses
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal VipPrice { get; set; }
    }
}
