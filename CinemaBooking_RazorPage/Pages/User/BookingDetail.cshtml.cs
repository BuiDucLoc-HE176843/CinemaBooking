using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CinemaBooking_RazorPage.Pages.User
{
    public class BookingDetailModel : PageModel
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
