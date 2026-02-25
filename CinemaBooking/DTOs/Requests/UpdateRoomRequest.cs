using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.DTOs.Requests
{
    public class UpdateRoomRequest
    {
        public int? TheaterId { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        public int? Rows { get; set; }
        public int? Columns { get; set; }

        public decimal? RegularPrice { get; set; }
        public decimal? VipPrice { get; set; }
    }
}
