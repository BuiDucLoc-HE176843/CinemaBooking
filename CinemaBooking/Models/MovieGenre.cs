using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaBooking.Models
{
    public class MovieGenre
    {
        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie? Movie { get; set; }

        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre? Genre { get; set; }
    }
}
