using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static Webshop.Pages.UserManagementModel;

namespace Webshop.Pages
{
    public class EditUserModel : PageModel
    {
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM users WHERE id = @id";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@id";
            paramA.Value = id;
            dbCommand.Parameters.Add(paramA);


                var reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    User = new User
                    {
                        Id = reader.GetInt32(0),
                        Gebruikersnaam = reader.GetString(1),
                        Wachtwoord = reader.GetString(2),
                        Email = reader.GetString(3)
                    };
                }
                else
                {
                    return NotFound();
                }
            dbConnection.Close();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string gebruikersnaam, string email)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "UPDATE users SET username = @username, email = @email WHERE id = @id";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@username";
            paramA.Value = gebruikersnaam;
            dbCommand.Parameters.Add(paramA);

            SqlParameter paramP = new SqlParameter();
            paramP.ParameterName = "@email";
            paramP.Value = email;
            dbCommand.Parameters.Add(paramP);

            SqlParameter paramN = new SqlParameter();
            paramN.ParameterName = "@id";
            paramN.Value = id;
            dbCommand.Parameters.Add(paramN);
            dbCommand.ExecuteNonQuery();

            dbConnection.Close();

            return RedirectToPage("/UserManagement");
           
        }
    }
}
