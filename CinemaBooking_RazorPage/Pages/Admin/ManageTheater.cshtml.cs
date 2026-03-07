using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class ManageTheaterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ManageTheaterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public List<TheaterResponse> Theaters { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Address { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CityName { get; set; }


        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 4;

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public async Task OnGetAsync()
        {
            var url = $"http://localhost:5237/api/Theaters?PageNumber={PageNumber}&PageSize={PageSize}";

            if (!string.IsNullOrEmpty(Name))
                url += $"&Name={Name}";

            if (!string.IsNullOrEmpty(Address))
                url += $"&Address={Address}";

            if (!string.IsNullOrEmpty(CityName))
                url += $"&CityName={CityName}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<ApiResponse<PagedData<TheaterResponse>>>(json, options);

                if (result != null && result.Success)
                {
                    Theaters = result.Data.Items;
                    TotalPages = result.Data.TotalPages;
                    TotalCount = result.Data.TotalCount;
                }
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
