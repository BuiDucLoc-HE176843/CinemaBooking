using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace CinemaBooking_RazorPage.Pages.PublicPage
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();

            var loginData = new
            {
                email = Email,
                password = Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("http://localhost:5237/api/Auth/login", content);

            var responseString = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            string message = root.GetProperty("message").GetString();

            if (!success)
            {
                ErrorMessage = message;
                return Page();
            }

            var data = root.GetProperty("data");
            string role = data.GetProperty("role").GetString();
            string token = data.GetProperty("token").GetString();

            // Lưu token vào Session
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("UserRole", role);

            // Điều hướng theo role
            if (role == "Admin")
            {
                return RedirectToPage("/Admin/ManageMovie");
            }
            else
            {
                return RedirectToPage("/PublicPage/Homepage");
            }
        }
    }
}
