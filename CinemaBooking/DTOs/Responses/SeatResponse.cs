using CinemaBooking.Enums;

namespace CinemaBooking.DTOs.Responses
{
    public class SeatResponse
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public SeatType Type { get; set; }

        public bool IsDisabled { get; set; }
    }
}
