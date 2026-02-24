namespace CinemaBooking.DTOs.Responses
{
    public class TheaterResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        public int CityId { get; set; }

        public string? CityName { get; set; }
    }
}
