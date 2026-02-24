using CinemaBooking.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int ShowtimeId { get; set; }
        [ForeignKey(nameof(ShowtimeId))]
        public Showtime? Showtime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        public ICollection<BookingSeat>? BookingSeats { get; set; }
    }
}
