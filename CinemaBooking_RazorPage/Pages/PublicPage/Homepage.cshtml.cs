using CinemaBooking_RazorPage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CinemaBooking_RazorPage.Pages.PublicPage
{
    public class HomepageModel : PageModel
    {
        private readonly IHttpClientFactory _factory;

        public Movie? MainFeatureMovie { get; set; }
        public List<Movie> Movies { get; set; } = new();
        public List<Movie> UpcomingMovies { get; set; } = new();

        public HomepageModel(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task OnGetAsync()
        {
            var client = _factory.CreateClient();

            var mainFeatureResponse = await client.GetFromJsonAsync<ApiResponse<PagedData<Movie>>>(
                "http://localhost:5237/api/Movies?IsMainFeature=true");

            if (mainFeatureResponse?.Data?.Items != null && mainFeatureResponse.Data.Items.Any())
            {
                MainFeatureMovie = mainFeatureResponse.Data.Items.First();
            }

            var response = await client.GetFromJsonAsync<ApiResponse<PagedData<Movie>>>(
                "http://localhost:5237/api/Movies?Status=1");

            if (response?.Data?.Items != null)
            {
                Movies = response.Data.Items;
            }

            var upcomingResponse = await client.GetFromJsonAsync<ApiResponse<PagedData<Movie>>>(
                "http://localhost:5237/api/Movies?Status=0");

            if (upcomingResponse?.Data?.Items != null)
            {
                UpcomingMovies = upcomingResponse.Data.Items;
            }
        }
    }
}
