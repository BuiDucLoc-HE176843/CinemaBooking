using CinemaBooking_RazorPage.DTOs.Requests;
using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.PublicPage
{
    public class MovieDetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public MovieDetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public MovieResponse? Movie { get; set; }

        public List<ReviewResponse> Reviews { get; set; } = new();

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }

        [BindProperty]
        public CreateReviewRequest ReviewInput { get; set; } = new();

        public async Task OnGet(int id)
        {
            // lấy movie
            var movieRes = await _httpClient.GetAsync($"http://localhost:5237/api/Movies?Id={id}");

            if (movieRes.IsSuccessStatusCode)
            {
                var json = await movieRes.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse<PagedData<MovieResponse>>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Movie = result?.Data?.Items?.FirstOrDefault();
            }

            // lấy review
            var reviewRes = await _httpClient.GetAsync($"http://localhost:5237/api/Reviews/movie/{id}");

            if (reviewRes.IsSuccessStatusCode)
            {
                var json = await reviewRes.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse<List<ReviewResponse>>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Reviews = result?.Data ?? new List<ReviewResponse>();

                ReviewCount = Reviews.Count;

                if (ReviewCount > 0)
                {
                    AverageRating = Math.Round(Reviews.Average(r => r.Rating), 1);
                }
            }
        }

        public async Task<IActionResult> OnPostReviewAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá";
                return RedirectToPage(new { id = ReviewInput.MovieId });
            }

            var json = JsonSerializer.Serialize(ReviewInput);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(
                "http://localhost:5237/api/Reviews",
                content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null && result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Có lỗi xảy ra";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể gửi đánh giá";
            }

            return RedirectToPage(new { id = ReviewInput.MovieId });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
