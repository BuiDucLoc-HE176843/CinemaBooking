using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.User
{
    public class BookingModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public BookingModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public MovieResponse? Movie { get; set; }

        public List<TheaterResponse> Theaters { get; set; } = new();

        public List<ShowtimeResponse> Showtimes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // MovieId

        [BindProperty(SupportsGet = true)]
        public int? TheaterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartTime { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndTime { get; set; }


        [BindProperty(SupportsGet = true)]
        public int? ShowtimeId { get; set; }
        public List<ShowtimeSeatResponse> Seats { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadMovie();
            await LoadTheaters();

            if (StartTime == null)
            {
                StartTime = DateTime.Today;
            }

            await LoadShowtimes();

            if (ShowtimeId.HasValue)
            {
                await LoadSeats();
            }
        }

        private async Task LoadSeats()
        {
            var url = $"http://localhost:5237/api/ShowtimeSeats?showtimeId={ShowtimeId}";

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<List<ShowtimeSeatResponse>>>(url);

            if (result != null && result.Success)
            {
                Seats = result.Data;
            }
        }

        private async Task LoadMovie()
        {
            var movieRes = await _httpClient.GetAsync($"http://localhost:5237/api/Movies?Id={Id}");

            if (!movieRes.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không thể tải thông tin phim");
                return;
            }

            var json = await movieRes.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<PagedData<MovieResponse>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Movie = result?.Data?.Items?.FirstOrDefault();
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

        private async Task LoadShowtimes()
        {
            var dateStart = StartTime?.ToString("yyyy-MM-dd");

            // chỉnh EndTime thành 23:59:59 của ngày được chọn
            var endOfDay = EndTime?.Date.AddDays(1).AddSeconds(-1);
            var dateEnd = endOfDay?.ToString("yyyy-MM-dd HH:mm:ss");

            var url = $"http://localhost:5237/api/Showtime?MovieId={Id}&StartTime={dateStart}&EndTime={dateEnd}";

            // chỉ thêm TheaterId nếu user chọn
            if (TheaterId.HasValue)
            {
                url += $"&TheaterId={TheaterId.Value}";
            }

            var result = await _httpClient
                .GetFromJsonAsync<ApiResponse<PagedData<ShowtimeResponse>>>(url);

            if (result != null && result.Success)
            {
                Showtimes = result.Data.Items;
            }
        }
    }
}
