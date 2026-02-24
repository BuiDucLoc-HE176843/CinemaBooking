using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class Showtime
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie? Movie { get; set; }

        public int RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        public DateTime ShowDateTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
