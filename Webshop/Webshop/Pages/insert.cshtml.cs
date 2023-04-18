using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Webshop.Pages
{
    public class insertModel : PageModel
    {
        [BindProperty, Required]
        public string Naam { get; set; }
        [BindProperty, Required]
        public int Prijs { get; set; }

        [BindProperty]
        public IFormFile Afbeelding { get; set; }

        public void OnGet()
        {
            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Afbeelding == null || Afbeelding.Length == 0)
                return Page();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", Afbeelding.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Afbeelding.CopyToAsync(fileStream);
            }


            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);
            string query = "INSERT INTO producten (naam, prijs, afbeelding) VALUES (@naam, @prijs, @afbeelding)";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();


            SqlParameter paramN = new SqlParameter();
            paramN.ParameterName = "@naam";
            paramN.Value = Naam;
            dbCommand.Parameters.Add(paramN);

            SqlParameter paramP = new SqlParameter();
            paramP.ParameterName = "@prijs";
            paramP.Value = Prijs;
            dbCommand.Parameters.Add(paramP);

            SqlParameter paramAfb = new SqlParameter();
            paramAfb.ParameterName = "@afbeelding";
            paramAfb.Value = Afbeelding.FileName.ToString();
            dbCommand.Parameters.Add(paramAfb);
            dbCommand.ExecuteNonQuery();
            dbConnection.Close();

            return RedirectToPage("Beheer");
        }
    }
}
