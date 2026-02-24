using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Ví dụ: "Hà Nội", "TP. Hồ Chí Minh"

        // Navigation
        public ICollection<Theater>? Theaters { get; set; }
    }
}
