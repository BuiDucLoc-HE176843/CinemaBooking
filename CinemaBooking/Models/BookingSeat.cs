using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class BookingSeat
    {
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        public int ShowtimeSeatId { get; set; }

        [ForeignKey(nameof(ShowtimeSeatId))]
        public ShowtimeSeat? ShowtimeSeat { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtBooking { get; set; }
    }
}
