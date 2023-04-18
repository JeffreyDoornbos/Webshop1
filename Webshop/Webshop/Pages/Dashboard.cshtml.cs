using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop.Pages
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
        }
    }
}
