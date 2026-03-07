using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class ManageRoomModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<TheaterResponse> Theaters { get; set; } = new();
        public List<RoomResponse> Rooms { get; set; } = new();

        [BindProperty]
        public int? SelectedTheaterId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 4;

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public ManageRoomModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task OnGetAsync()
        {
            await LoadTheaters();
            await OnPostSearchAsync();
        }

        public async Task<IActionResult> OnGetPageAsync(int theaterId, int pageNumber = 1)
        {
            SelectedTheaterId = theaterId;
            PageNumber = pageNumber;

            await LoadTheaters();
            await LoadRooms();

            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            PageNumber = 1;

            await LoadTheaters();
            await LoadRooms();

            return Page();
        }

        private async Task LoadRooms()
        {
            if (SelectedTheaterId == null)
            {
                SelectedTheaterId = 1;
            }

            var url = $"http://localhost:5237/api/Room/{SelectedTheaterId}?PageNumber={PageNumber}&PageSize={PageSize}";

            var result = await _httpClient.GetFromJsonAsync<ApiResponse<PagedData<RoomResponse>>>(url);

            if (result != null && result.Success)
            {
                Rooms = result.Data.Items;
                TotalPages = result.Data.TotalPages;
                TotalCount = result.Data.TotalCount;
            }
        }

        private async Task LoadTheaters()
        {
            var url = "http://localhost:5237/api/Theaters?PageSize=50";

            var result = await _httpClient.GetFromJsonAsync<ApiResponse<PagedData<TheaterResponse>>>(url);

            if (result != null && result.Success)
            {
                Theaters = result.Data.Items;
            }
        }
    }
}
