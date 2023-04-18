using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Webshop.Pages
{
    public class AddUserModel : PageModel
    {



        [BindProperty]
        [Required]
        [Display(Name = "Gebruikersnaam")]
        public string? Gebruikersnaam { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string? Wachtwoord { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public void OnGet()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }

        }

            public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "INSERT INTO users (username, password, email) VALUES (@username, @password, @email)";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@username";
            paramA.Value = Gebruikersnaam;
            dbCommand.Parameters.Add(paramA);

            SqlParameter paramN = new SqlParameter();
            paramN.ParameterName = "@password";
            paramN.Value = Wachtwoord;
            dbCommand.Parameters.Add(paramN);

            SqlParameter paramP = new SqlParameter();
            paramP.ParameterName = "@email";
            paramP.Value = Email;
            dbCommand.Parameters.Add(paramP);

            dbCommand.ExecuteNonQuery();
            dbConnection.Close();

            return RedirectToPage("/Beheer");
            
        }
    }
}
