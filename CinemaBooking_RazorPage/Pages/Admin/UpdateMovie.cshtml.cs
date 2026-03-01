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


        [BindProperty]
        public Movie Movie2 { get; set; }

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
                Movie2 = Movie;
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(Movie2.Title ?? ""), "Title");
            content.Add(new StringContent(Movie2.Description ?? ""), "Description");
            content.Add(new StringContent(Movie2.ReleaseDate.ToString("o")), "ReleaseDate");
            content.Add(new StringContent(Movie2.DurationMinutes.ToString()), "DurationMinutes");
            content.Add(new StringContent(Movie2.Status.ToString()), "Status");

            foreach (var genreId in SelectedGenres)
            {
                content.Add(new StringContent(genreId.ToString()), "GenreIds");
            }

            if (PosterFile != null)
            {
                var stream = PosterFile.OpenReadStream();
                content.Add(new StreamContent(stream), "PosterFile", PosterFile.FileName);
            }

            if (TrailerFile != null)
            {
                var stream = TrailerFile.OpenReadStream();
                content.Add(new StreamContent(stream), "TrailerFile", TrailerFile.FileName);
            }

            var response = await _httpClient.PutAsync($"http://localhost:5237/api/Movies/{id}", content);

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, options);

            if (result != null && result.Success)
            {
                SuccessMessage = result.Message;
                return RedirectToPage(new { id = id });
            }
            else
            {
                ErrorMessage = result?.Message ?? "Có lỗi xảy ra";
                await OnGet(id);
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
