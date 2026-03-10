using CinemaBooking.Configuration;

namespace CinemaBooking.DTOs.Requests
{
    public class ShowtimeFilterRequest : PaginationRequest
    {
        public int? Id { get; set; }
        public int? MovieId { get; set; }
        public int? RoomId { get; set; }
    }
}
