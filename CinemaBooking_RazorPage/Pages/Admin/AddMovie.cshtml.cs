using CinemaBooking_RazorPage.DTOs.Responses;
using CinemaBooking_RazorPage.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class AddMovieModel : PageModel
    {
        [BindProperty]
        public CreateMovieRequest Movie { get; set; }


        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public List<GenreResponse> AllGenres { get; set; } = new();


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

                var result = JsonSerializer.Deserialize<ApiResponse<List<GenreResponse>>>(json, options);
                AllGenres = result?.Data ?? new List<GenreResponse>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGet(); // reload genres
                return Page();
            }

            using var content = new MultipartFormDataContent();

            // ===== TEXT DATA =====
            content.Add(new StringContent(Movie.Title), nameof(Movie.Title));

            if (!string.IsNullOrEmpty(Movie.Description))
                content.Add(new StringContent(Movie.Description), nameof(Movie.Description));

            if (Movie.ReleaseDate.HasValue)
                content.Add(
                    new StringContent(Movie.ReleaseDate.Value.ToString("o")),
                    nameof(Movie.ReleaseDate));

            content.Add(
                new StringContent(Movie.DurationMinutes.ToString()),
                nameof(Movie.DurationMinutes));

            if (!string.IsNullOrEmpty(Movie.Status))
                content.Add(
                    new StringContent(Movie.Status),
                    nameof(Movie.Status));

            content.Add(
                new StringContent(Movie.IsMainFeature.ToString()),
                nameof(Movie.IsMainFeature));

            // ===== GENRES =====
            if (Movie.GenreIds != null && Movie.GenreIds.Any())
            {
                foreach (var genreId in Movie.GenreIds)
                {
                    content.Add(
                        new StringContent(genreId.ToString()),
                        "GenreIds"); // phải đúng tên API nhận
                }
            }

            // ===== POSTER =====
            if (Movie.PosterFile != null)
            {
                var streamContent = new StreamContent(Movie.PosterFile.OpenReadStream());
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(Movie.PosterFile.ContentType);

                content.Add(
                    streamContent,
                    nameof(Movie.PosterFile),
                    Movie.PosterFile.FileName);
            }

            // ===== TRAILER =====
            if (Movie.TrailerFile != null)
            {
                var streamContent = new StreamContent(Movie.TrailerFile.OpenReadStream());
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(Movie.TrailerFile.ContentType);

                content.Add(
                    streamContent,
                    nameof(Movie.TrailerFile),
                    Movie.TrailerFile.FileName);
            }

            // ===== CALL API =====
            var response = await _httpClient.PostAsync(
                "http://localhost:5237/api/Movies",
                content);

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, options);

            if (result != null && result.Success)
            {
                SuccessMessage = result.Message;
                return RedirectToPage();
            }

            ErrorMessage = result?.Message ?? "Có lỗi xảy ra";
            await OnGet(); // reload genres
            return Page();
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
