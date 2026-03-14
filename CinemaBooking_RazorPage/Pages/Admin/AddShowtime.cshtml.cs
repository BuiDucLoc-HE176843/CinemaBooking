using CinemaBooking_RazorPage.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class AddShowtimeModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<TheaterResponse> Theaters { get; set; } = new();
        public List<RoomResponse> Rooms { get; set; } = new();
        public List<MovieResponse> Movies { get; set; } = new();

        [BindProperty]
        public int? SelectedTheaterId { get; set; }

        [BindProperty]
        public CreateShowtimeRequest Input { get; set; } = new();

        public AddShowtimeModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet()
        {
            await LoadTheaters();
        }

        public async Task<IActionResult> OnPostLoadRoomsAsync()
        {
            await LoadTheaters();

            if (SelectedTheaterId != null)
            {
                await LoadRooms();
                await LoadMovies();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5237/api/Showtime",
                Input);

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse != null && apiResponse.Success)
            {
                TempData["SuccessMessage"] = apiResponse.Message;
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = apiResponse?.Message ?? "Có lỗi xảy ra";

            await LoadTheaters();
            await LoadRooms();
            await LoadMovies();
            return Page();
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

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageShowtime");
        }
    }
}
