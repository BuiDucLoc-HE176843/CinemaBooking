using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.DTOs.Requests
{
    public class UpdateTheaterRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        public int? CityId { get; set; }
    }
}
