using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class UpdateRoomModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateRoomModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public UpdateRoomRequest Input { get; set; } = new();

        [BindProperty]
        public int RoomId { get; set; }

        public async Task OnGet(int id)
        {
            RoomId = id;

            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Room/room/{id}");
            var json = await response.Content.ReadAsStringAsync();

            var roomData = JsonSerializer.Deserialize<ApiResponse<RoomResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (roomData != null && roomData.Success && roomData.Data != null)
            {
                var room = roomData.Data;

                Input = new UpdateRoomRequest
                {
                    TheaterId = room.TheaterId,
                    Name = room.Name,
                    Rows = room.Rows,
                    Columns = room.Columns,
                    RegularPrice = room.RegularPrice,
                    VipPrice = room.VipPrice
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                $"http://localhost:5237/api/Room/{RoomId}",
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
                TempData["ErrorMessage"] = result?.Message ?? "Cập nhật phòng thất bại";
            }

            return Page();
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
