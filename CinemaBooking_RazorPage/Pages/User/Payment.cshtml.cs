using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.User
{
    public class PaymentModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public PaymentModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public BookingResponse Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Booking/{bookingId}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không gọi được API");
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(json, options);

            if (result == null || !result.Success)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Lỗi dữ liệu");
                return Page();
            }

            Booking = result.Data;

            return Page();
        }
    }
}
