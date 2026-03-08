using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CinemaBooking_RazorPage.Pages.Admin
{
    public class EditSeatModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Admin/ManageRoom");
        }
    }
}
