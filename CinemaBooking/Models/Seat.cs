using CinemaBooking.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }

        [Required]
        public SeatType Type { get; set; } = SeatType.Regular;

        public bool IsDisabled { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<ShowtimeSeat>? ShowtimeSeats { get; set; }
    }
}
