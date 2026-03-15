using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CinemaBooking_RazorPage.Pages.PublicPage
{
    public class MovieDetailModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/PublicPage/Homepage");
        }
    }
}
