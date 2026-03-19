using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.User
{
    public class BookingHistoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public BookingHistoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public PagedData<BookingResponse> Bookings { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 3;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"http://localhost:5237/api/Booking/me?pageNumber={PageNumber}&pageSize={PageSize}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Không tải được dữ liệu");
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedData<BookingResponse>>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (apiResponse?.Success == true)
            {
                Bookings = apiResponse.Data;
            }

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
