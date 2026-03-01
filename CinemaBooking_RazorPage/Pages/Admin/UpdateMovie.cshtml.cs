using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;


namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class UpdateMovieModel : PageModel
    {
        public MovieResponse Movie { get; set; } = new();

        [BindProperty]
        public UpdateMovieRequest Input { get; set; } = new();

        public List<GenreResponse> AllGenres { get; set; } = new();
        public List<int> MovieGenreIds { get; set; } = new();


        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }


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

                var result = JsonSerializer.Deserialize<ApiResponse<PagedData<MovieResponse>>>(json, options);

                Movie = result?.Data?.Items?.FirstOrDefault();
                if (Movie != null)
                {
                    Input = new UpdateMovieRequest
                    {
                        Title = Movie.Title,
                        Description = Movie.Description,
                        ReleaseDate = Movie.ReleaseDate,
                        DurationMinutes = Movie.DurationMinutes,
                        Status = Movie.Status.ToString(),
                        IsMainFeature = Movie.IsMainFeature
                    };
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

            // Lấy thể loại theo phim
            var movieGenreResponse = await _httpClient.GetAsync($"http://localhost:5237/api/Genres/movie/{id}");

            if (movieGenreResponse.IsSuccessStatusCode)
            {
                var json = await movieGenreResponse.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ApiResponse<List<GenreResponse>>>(json, options);

                MovieGenreIds = result?.Data?.Select(g => g.Id).ToList() ?? new List<int>();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            using var content = new MultipartFormDataContent();

            // ===== Basic Fields =====
            content.Add(new StringContent(Input.Title ?? string.Empty), nameof(Input.Title));
            content.Add(new StringContent(Input.Description ?? string.Empty), nameof(Input.Description));

            if (Input.ReleaseDate.HasValue)
            {
                content.Add(
                    new StringContent(Input.ReleaseDate.Value.ToString("o")),
                    nameof(Input.ReleaseDate));
            }

            if (Input.DurationMinutes.HasValue)
            {
                content.Add(
                    new StringContent(Input.DurationMinutes.Value.ToString()),
                    nameof(Input.DurationMinutes));
            }

            content.Add(
                new StringContent(Input.Status ?? string.Empty),
                nameof(Input.Status));

            content.Add(
                new StringContent(Input.IsMainFeature.ToString()),
                nameof(Input.IsMainFeature));

            // ===== Genres =====
            if (Input.GenreIds != null && Input.GenreIds.Any())
            {
                foreach (var genreId in Input.GenreIds)
                {
                    content.Add(
                        new StringContent(genreId.ToString()),
                        "GenreIds"); // phải đúng tên API nhận
                }
            }

            // ===== Poster File =====
            if (Input.PosterFile != null)
            {
                var streamContent = new StreamContent(Input.PosterFile.OpenReadStream());
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(Input.PosterFile.ContentType);

                content.Add(
                    streamContent,
                    nameof(Input.PosterFile),
                    Input.PosterFile.FileName);
            }

            // ===== Trailer File =====
            if (Input.TrailerFile != null)
            {
                var streamContent = new StreamContent(Input.TrailerFile.OpenReadStream());
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(Input.TrailerFile.ContentType);

                content.Add(
                    streamContent,
                    nameof(Input.TrailerFile),
                    Input.TrailerFile.FileName);
            }

            // ===== Call API =====
            var response = await _httpClient.PutAsync(
                $"http://localhost:5237/api/Movies/{id}",
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
                return RedirectToPage(new { id });
            }

            ErrorMessage = result?.Message ?? "Có lỗi xảy ra";
            await OnGet(id);
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
