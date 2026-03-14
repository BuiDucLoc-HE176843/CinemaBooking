using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using CinemaBooking_RazorPage.DTOs.Responses;

namespace CinemaBooking_RazorPage.Pages.PublicPage
{
    public class MoviesListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public MoviesListModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public List<MovieResponse> Movies { get; set; } = new();
        public List<GenreResponse> AllGenres { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // giữ filter
        [BindProperty(SupportsGet = true)]
        public string? Title { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ReleaseDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GenreId { get; set; }

        public async Task OnGet(int pageNumber = 1)
        {
            CurrentPage = pageNumber;

            var query = new List<string>();

            if (!string.IsNullOrEmpty(Title))
                query.Add($"Title={Title}");

            if (ReleaseDate != null)
                query.Add($"ReleaseDate={ReleaseDate:yyyy-MM-dd}");

            if (Status != null)
                query.Add($"Status={Status}");

            if (GenreId != null)
                query.Add($"GenreId={GenreId}");

            query.Add($"PageNumber={pageNumber}");
            query.Add("PageSize=10");

            var url = "http://localhost:5237/api/Movies?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<ApiResponse<PagedData<MovieResponse>>>(json, options);

                if (result != null && result.Success)
                {
                    Movies = result.Data.Items;
                    TotalPages = result.Data.TotalPages;
                }
            }

            // Lấy toàn bộ thể loại
            var genreResponse = await _httpClient.GetAsync("http://localhost:5237/api/Genres");

            if (genreResponse.IsSuccessStatusCode)
            {
                var json = await genreResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ApiResponse<List<GenreResponse>>>(json, options);

                AllGenres = result?.Data ?? new List<GenreResponse>();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
