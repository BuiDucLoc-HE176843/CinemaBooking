using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public int TheaterId { get; set; }
        [ForeignKey(nameof(TheaterId))]
        public Theater? Theater { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int Rows { get; set; } = 8;
        public int Columns { get; set; } = 8;

        [Column(TypeName = "decimal(10,2)")]
        public decimal RegularPrice { get; set; } = 90000m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal VipPrice { get; set; } = 150000m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Seat>? Seats { get; set; }
        public ICollection<Showtime>? Showtimes { get; set; }
    }
}
