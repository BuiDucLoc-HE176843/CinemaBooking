namespace CinemaBooking_RazorPage.Model
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DurationMinutes { get; set; }
        public int Status { get; set; }
        public string PosterUrl { get; set; }
        public string TrailerUrl { get; set; }
        public List<string> Genres { get; set; }
    }
}
