using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class EditSeatModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditSeatModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public List<SeatResponse> Seats { get; set; } = new();

        public SeatResponse? SelectedSeat { get; set; }

        public int MaxRow { get; set; }
        public int MaxColumn { get; set; }

        [BindProperty]
        public SeatUpdateRequest Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? SeatId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // RoomId

        public async Task OnGet()
        {
            await LoadRoomSeats();

            if (SeatId.HasValue)
            {
                await LoadSeat(SeatId.Value);
            }
        }

        private async Task LoadRoomSeats()
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Seats/room/{Id}");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<List<SeatResponse>>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true)
            {
                Seats = result.Data;
                MaxRow = Seats.Max(x => x.RowNumber);
                MaxColumn = Seats.Max(x => x.ColumnNumber);
            }
        }

        private async Task LoadSeat(int seatId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Seats/{seatId}");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<SeatResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true)
            {
                SelectedSeat = result.Data;

                Input.Type = (DTOs.Requests.SeatType)SelectedSeat.Type;
                Input.IsDisabled = SelectedSeat.IsDisabled;
            }
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            if (!SeatId.HasValue)
            {
                TempData["ErrorMessage"] = "Không tìm thấy ghế";
                return RedirectToPage(new { id = Id });
            }

            var json = JsonSerializer.Serialize(Input);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                $"http://localhost:5237/api/Seats/{SeatId}",
                content);

            var responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Cập nhật thất bại";
            }

            return RedirectToPage(new { id = Id, seatId = SeatId });
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageRoom");
        }
    }
}