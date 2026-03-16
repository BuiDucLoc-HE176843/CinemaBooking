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

        [BindProperty(SupportsGet = true)]
        public List<int> ShowtimeSeatIds { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt vé";
                return RedirectToPage("/PublicPage/Login");
            }

            if (!ShowtimeId.HasValue || ShowtimeSeatIds == null || !ShowtimeSeatIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ghế";
                return RedirectToPage(new
                {
                    Id,
                    TheaterId,
                    StartTime,
                    EndTime,
                    ShowtimeId
                });
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                showtimeId = ShowtimeId.Value,
                showtimeSeatIds = ShowtimeSeatIds
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5237/api/Booking",
                request
            );

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();

            // lỗi http hoặc api
            if (!response.IsSuccessStatusCode || result == null)
            {
                TempData["ErrorMessage"] = "Không thể tạo booking";
                return RedirectToPage(new
                {
                    Id,
                    TheaterId,
                    StartTime,
                    EndTime,
                    ShowtimeId
                });
            }

            // API trả success = false
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;

                return RedirectToPage(new
                {
                    Id,
                    TheaterId,
                    StartTime,
                    EndTime,
                    ShowtimeId
                });
            }

            // Thành công
            var bookingId = result.Data.GetProperty("bookingId").GetInt32();

            return RedirectToPage("/User/Payment", new { bookingId });
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
