using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CinemaBooking_RazorPage.DTOs.Responses;


namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class ManageShowtimeModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<TheaterResponse> Theaters { get; set; } = new();
        public List<RoomResponse> Rooms { get; set; } = new();
        public List<MovieResponse> Movies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? SelectedTheaterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedRoomId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedMovieId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartTime { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndTime { get; set; }

        public List<ShowtimeResponse> Showtimes { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }

        public ManageShowtimeModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet(int pageNumber = 1)
        {
            PageNumber = pageNumber;

            await LoadTheaters();
            await LoadMovies();
            await LoadShowtimes();
        }

        public async Task<IActionResult> OnPostAsync(int pageNumber = 1)
        {
            PageNumber = pageNumber;

            await LoadTheaters();

            await LoadRooms();
            await LoadMovies();
            await LoadShowtimes();

            return Page();
        }

        private async Task LoadShowtimes()
        {
            var url = $"http://localhost:5237/api/Showtime?PageNumber={PageNumber}&PageSize=4";

            if (SelectedRoomId != null)
                url += $"&RoomId={SelectedRoomId}";

            if (SelectedMovieId != null)
                url += $"&MovieId={SelectedMovieId}";

            if (StartTime != null)
                url += $"&StartTime={StartTime:yyyy-MM-ddTHH:mm}";

            if (EndTime != null)
                url += $"&EndTime={EndTime:yyyy-MM-ddTHH:mm}";

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<PagedData<ShowtimeResponse>>>(url);

            if (result != null && result.Success)
            {
                Showtimes = result.Data.Items;
                TotalPages = result.Data.TotalPages;
            }
        }

        private async Task LoadTheaters()
        {
            var url = "http://localhost:5237/api/Theaters?PageSize=50";

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<PagedData<TheaterResponse>>>(url);

            if (result != null && result.Success)
            {
                Theaters = result.Data.Items;
            }
        }

        private async Task LoadMovies()
        {
            var url = "http://localhost:5237/api/Movies?PageSize=50";

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<PagedData<MovieResponse>>>(url);

            if (result != null && result.Success)
            {
                Movies = result.Data.Items;
            }
        }

        private async Task LoadRooms()
        {
            if (SelectedTheaterId == null) return;

            var url = $"http://localhost:5237/api/Room/{SelectedTheaterId}";

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<PagedData<RoomResponse>>>(url);

            if (result != null && result.Success)
            {
                Rooms = result.Data.Items;
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
