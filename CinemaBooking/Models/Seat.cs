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

        public int RowNumber { get; set; }     // 1..8
        public int ColumnNumber { get; set; }  // 1..8

        [Required]
        public SeatType Type { get; set; } = SeatType.Regular;

        [Required]
        public SeatStatus Status { get; set; } = SeatStatus.Available;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<BookingSeat>? BookingSeats { get; set; }
    }
}
