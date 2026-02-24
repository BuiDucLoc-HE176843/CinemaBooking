using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;  // Ví dụ: "Hành động", "Hài", "Kinh dị", "Tình cảm"

        // Navigation (many-to-many)
        public ICollection<MovieGenre>? MovieGenres { get; set; }
    }
}
