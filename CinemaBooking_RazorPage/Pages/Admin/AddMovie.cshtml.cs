using CinemaBooking_RazorPage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class AddMovieModel : PageModel
    {
        [BindProperty]
        public Movie Movie { get; set; }

        [BindProperty]
        public IFormFile? PosterFile { get; set; }

        [BindProperty]
        public IFormFile? TrailerFile { get; set; }

        [BindProperty]
        public List<int> SelectedGenres { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public List<Genre> AllGenres { get; set; } = new();


        private readonly HttpClient _httpClient;

        public AddMovieModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task OnGet()
        {
            var genreResponse = await _httpClient.GetAsync("http://localhost:5237/api/Genres");

            if (genreResponse.IsSuccessStatusCode)
            {
                var json = await genreResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ApiResponse<List<Genre>>>(json, options);
                AllGenres = result?.Data ?? new List<Genre>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using var content = new MultipartFormDataContent();

            // ===== TEXT DATA =====
            content.Add(new StringContent(Movie.Title ?? ""), "Title");
            content.Add(new StringContent(Movie.Description ?? ""), "Description");
            content.Add(new StringContent(Movie.ReleaseDate.ToString("o")), "ReleaseDate");
            content.Add(new StringContent(Movie.DurationMinutes.ToString()), "DurationMinutes");
            content.Add(new StringContent(Movie.Status.ToString()), "Status");
            content.Add(new StringContent(Movie.IsMainFeature.ToString()), "IsMainFeature");

            // ===== GENRES =====
            foreach (var genreId in SelectedGenres)
            {
                content.Add(new StringContent(genreId.ToString()), "GenreIds");
            }

            // ===== POSTER =====
            if (PosterFile != null)
            {
                var stream = PosterFile.OpenReadStream();
                content.Add(
                    new StreamContent(stream),
                    "PosterFile",
                    PosterFile.FileName
                );
            }

            // ===== TRAILER =====
            if (TrailerFile != null)
            {
                var stream = TrailerFile.OpenReadStream();
                content.Add(
                    new StreamContent(stream),
                    "TrailerFile",
                    TrailerFile.FileName
                );
            }

            // ===== CALL API (POST thay vì PUT) =====
            var response = await _httpClient.PostAsync(
                "http://localhost:5237/api/Movies",
                content
            );

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, options);

            if (result != null && result.Success)
            {
                SuccessMessage = result.Message;

                // giống behavior add thông thường
                return RedirectToPage();
            }
            else
            {
                ErrorMessage = result?.Message ?? "Có lỗi xảy ra";
                await OnGet(); // reload genres
                return Page();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageMovie");
        }
    }
}
