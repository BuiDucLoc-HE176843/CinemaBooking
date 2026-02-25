using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.DTOs.Requests
{
    public class CreateRoomRequest
    {
        [Required]
        public int TheaterId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int? Rows { get; set; }
        public int? Columns { get; set; }

        public decimal? RegularPrice { get; set; }
        public decimal? VipPrice { get; set; }
    }
}
