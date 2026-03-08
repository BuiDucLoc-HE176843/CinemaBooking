using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;

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
        public int MaxRow { get; set; }
        public int MaxColumn { get; set; }

        public async Task OnGet(int id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Seats/room/{id}");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<SeatResponse>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true)
            {
                Seats = result.Data;
                MaxRow = Seats.Max(x => x.RowNumber);
                MaxColumn = Seats.Max(x => x.ColumnNumber);
            }
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageRoom");
        }
    }
}