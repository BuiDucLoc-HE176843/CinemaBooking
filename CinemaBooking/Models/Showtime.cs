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

        public DateTime StartTime { get; set; }  // Thời gian bắt đầu chiếu
        public DateTime EndTime { get; set; }    // Thời gian kết thúc chiếu

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<ShowtimeSeat>? ShowtimeSeats { get; set; }
    }
}
