using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class AddRoomModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AddRoomModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public CreateRoomRequest Input { get; set; } = new();

        public List<TheaterResponse> Theaters { get; set; } = new();

        public async Task OnGet()
        {
            await LoadTheaters();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:5237/api/Room",
                content);

            var responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null && result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Tạo phòng thất bại";
            }

            return RedirectToPage(); // reload lại AddRoom
        }

        private async Task LoadTheaters()
        {
            var response = await _httpClient.GetAsync(
                "http://localhost:5237/api/Theaters?PageSize=50");

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<PagedData<TheaterResponse>>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Theaters = data?.Data?.Items ?? new List<TheaterResponse>();
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageRoom");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
