using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.User
{
    public class BookingDetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public BookingDetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public BookingResponse Booking { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Booking/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Không gọi được API";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(json, options);

            if (apiResponse == null || !apiResponse.Success)
            {
                ErrorMessage = apiResponse?.Message ?? "Có lỗi xảy ra";
                return Page();
            }

            Booking = apiResponse.Data;
            return Page();
        }
    }
}
