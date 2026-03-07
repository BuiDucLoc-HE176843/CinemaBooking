using System.ComponentModel.DataAnnotations;

namespace CinemaBooking_RazorPage.DTOs.Requests
{
    public class CreateRoomRequest
    {
        public int TheaterId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int? Rows { get; set; }
        public int? Columns { get; set; }

        public decimal? RegularPrice { get; set; }
        public decimal? VipPrice { get; set; }
    }
}
