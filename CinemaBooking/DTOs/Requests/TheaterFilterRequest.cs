using CinemaBooking.Configuration;

namespace CinemaBooking.DTOs.Requests
{
    public class TheaterFilterRequest : PaginationRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public int? CityId { get; set; }
        public string? CityName { get; set; }
    }
}
