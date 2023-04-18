using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Webshop.Pages
{
    public class UserManagementModel : PageModel
    {

        public class User
        {
            public int Id { get; set; }
            public string? Gebruikersnaam { get; set; }
            public string? Wachtwoord { get; set; }
            public string? Email { get; set; }
        }


        public List<User> Users { get; set; }



        public async Task OnGetAsync()
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

            string query = "SELECT * FROM users";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

                var reader = dbCommand.ExecuteReader();

                Users = new List<User>();

                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Gebruikersnaam = reader.GetString(1),
                        Wachtwoord = reader.GetString(2),
                        Email = reader.GetString(3)
                    };

                    Users.Add(user);
                }
            dbConnection.Close();
        }


        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "DELETE FROM users WHERE id = @id";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();


            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = id;
            dbCommand.Parameters.Add(param);

                dbCommand.ExecuteNonQuery();

                return RedirectToPage("/UserManagement");
            
        }
    }
}
