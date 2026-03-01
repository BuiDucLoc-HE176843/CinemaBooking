using CinemaBooking_RazorPage.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class ManageMovieModel : PageModel
    {
        public List<MovieResponse> Movies { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 4;


        [BindProperty(SupportsGet = true)]
        public string? Title { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ReleaseDate { get; set; }


        public async Task OnGet(int pageNumber = 1)
        {
            using (HttpClient client = new HttpClient())
            {
                //CurrentPage = pageNumber;

                //var response = await client.GetAsync(
                //    $"http://localhost:5237/api/Movies?PageNumber={CurrentPage}&PageSize={PageSize}");

                CurrentPage = pageNumber;

                var queryParams = new List<string>
                {
                    $"PageNumber={CurrentPage}",
                    $"PageSize={PageSize}"
                };

                if (!string.IsNullOrEmpty(Title))
                    queryParams.Add($"Title={Title}");

                if (Status.HasValue)
                    queryParams.Add($"Status={Status.Value}");

                if (ReleaseDate.HasValue)
                    queryParams.Add($"ReleaseDate={ReleaseDate.Value:yyyy-MM-dd}");

                var url = "http://localhost:5237/api/Movies?" + string.Join("&", queryParams);

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = JsonSerializer.Deserialize<ApiResponse<PagedData<MovieResponse>>>(jsonString, options);

                    if (result != null && result.Success)
                    {
                        Movies = result.Data.Items;
                        TotalPages = result.Data.TotalPages;
                        TotalCount = result.Data.TotalCount;
                    }
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
