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

        public async Task<JsonResult> OnGetCheckStatusAsync(int bookingId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Booking/{bookingId}");

            // 1. Kiểm tra lỗi kết nối hoặc lỗi server (500, 404, v.v.)
            if (!response.IsSuccessStatusCode)
            {
                return new JsonResult(new { status = "Error" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(json, options);

            // 2. Kiểm tra nếu result null (lỗi parse) hoặc API báo success = false
            // Ví dụ trường hợp: { "success": false, "message": "Booking không tồn tại", "data": null }
            if (result == null || !result.Success || result.Data == null)
            {
                return new JsonResult(new { status = "NotFound" });
            }

            // 3. Trường hợp thành công và có dữ liệu
            // Trả về đúng trạng thái từ API (Pending, Paid, Cancelled)
            return new JsonResult(new { status = result.Data.Status });
        }
    }
}
