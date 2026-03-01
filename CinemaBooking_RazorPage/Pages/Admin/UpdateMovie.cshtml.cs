using CinemaBooking_RazorPage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;


namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class UpdateMovieModel : PageModel
    {
        public Movie Movie { get; set; }

        public List<Genre> AllGenres { get; set; } = new();
        public List<int> MovieGenreIds { get; set; } = new();

        private readonly HttpClient _httpClient;

        public UpdateMovieModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task OnGet(int id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5237/api/Movies?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<ApiResponse<PagedData<Movie>>>(json, options);

                Movie = result?.Data?.Items?.FirstOrDefault();
            }

            // Lấy toàn bộ thể loại
            var genreResponse = await _httpClient.GetAsync("http://localhost:5237/api/Genres");

            if (genreResponse.IsSuccessStatusCode)
            {
                var json = await genreResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ApiResponse<List<Genre>>>(json, options);
                AllGenres = result?.Data ?? new List<Genre>();
            }

            // Lấy thể loại theo phim
            var movieGenreResponse = await _httpClient.GetAsync($"http://localhost:5237/api/Genres/movie/{id}");

            if (movieGenreResponse.IsSuccessStatusCode)
            {
                var json = await movieGenreResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ApiResponse<List<Genre>>>(json, options);

                MovieGenreIds = result?.Data?.Select(g => g.Id).ToList() ?? new List<int>();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
