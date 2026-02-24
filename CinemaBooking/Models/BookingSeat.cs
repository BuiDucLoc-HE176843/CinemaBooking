using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class BookingSeat
    {
        public int BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        public int SeatId { get; set; }
        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtBooking { get; set; }
    }
}
