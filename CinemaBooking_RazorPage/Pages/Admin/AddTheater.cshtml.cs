using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class AddTheaterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AddTheaterModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public CreateTheaterRequest Input { get; set; } = new();

        public List<CityResponse> Cities { get; set; } = new();

        public async Task OnGet()
        {
            await LoadCities();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5237/api/Theaters", content);

            var responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<TheaterResponse>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null && result.Success)
            {
                TempData["SuccessMessage"] = result.Message;

                // redirect để tránh submit lại form
                return RedirectToPage("/Admin/AddTheater");
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Có lỗi xảy ra";
            }

            await LoadCities();

            return Page();
        }

        private async Task LoadCities()
        {
            var cityResponse = await _httpClient.GetAsync("http://localhost:5237/api/Cities");

            var cityJson = await cityResponse.Content.ReadAsStringAsync();

            var cityData = JsonSerializer.Deserialize<ApiResponse<List<CityResponse>>>(cityJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Cities = cityData?.Data ?? new List<CityResponse>();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageTheater");
        }
    }
}
