using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class UpdateTheaterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateTheaterModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public UpdateTheaterRequest Input { get; set; } = new();

        public List<CityResponse> Cities { get; set; } = new();

        public int TheaterId { get; set; }

        public async Task OnGet(int id)
        {
            TheaterId = id;

            // lấy theater
            var theaterResponse = await _httpClient.GetAsync($"http://localhost:5237/api/Theaters?Id={id}");
            var theaterJson = await theaterResponse.Content.ReadAsStringAsync();

            var theaterData = JsonSerializer.Deserialize<ApiResponse<PagedData<TheaterResponse>>>(theaterJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var theater = theaterData?.Data?.Items?.FirstOrDefault();

            if (theater != null)
            {
                Input = new UpdateTheaterRequest
                {
                    Name = theater.Name,
                    Address = theater.Address,
                    CityId = theater.CityId
                };
            }

            // lấy cities
            var cityResponse = await _httpClient.GetAsync("http://localhost:5237/api/Cities");
            var cityJson = await cityResponse.Content.ReadAsStringAsync();

            var cityData = JsonSerializer.Deserialize<ApiResponse<List<CityResponse>>>(cityJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Cities = cityData?.Data ?? new List<CityResponse>();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            TheaterId = id;

            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5237/api/Theaters/{id}", content);

            var responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<TheaterResponse>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null && result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Có lỗi xảy ra";
            }

            // load lại cities
            var cityResponse = await _httpClient.GetAsync("http://localhost:5237/api/Cities");
            var cityJson = await cityResponse.Content.ReadAsStringAsync();

            var cityData = JsonSerializer.Deserialize<ApiResponse<List<CityResponse>>>(cityJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Cities = cityData?.Data ?? new List<CityResponse>();

            return Page();
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
