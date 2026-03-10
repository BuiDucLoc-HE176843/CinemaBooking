using CinemaBooking.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class ShowtimeSeat
    {
        [Key]
        public int Id { get; set; }

        public int ShowtimeId { get; set; }
        [ForeignKey(nameof(ShowtimeId))]
        public Showtime? Showtime { get; set; }

        public int SeatId { get; set; }
        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }

        public SeatStatus Status { get; set; } = SeatStatus.Available;
        public bool IsDeleted { get; set; } = false;

        public ICollection<BookingSeat>? BookingSeats { get; set; }
    }
}
