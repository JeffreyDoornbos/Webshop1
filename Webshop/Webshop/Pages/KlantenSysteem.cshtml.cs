using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Webshop.Pages
{
    public class KlantenSysteemModel : PageModel
    {
        public List<Klant> Klanten { get; set; }

        public void OnGet()
        {

            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
            Klanten = new List<Klant>();

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";

            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM klanten";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            using (var reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var klant = new Klant
                    {
                        ID = reader.GetInt32(0),
                        Naam = reader.GetString(1),
                        Email = reader.GetString(2),
                        Postcode = reader.GetString(3),
                        Plaats = reader.GetString(4)
                    };
                    Klanten.Add(klant);
                }
            }
        }
    }

    public class Klant
    {
        public int ID { get; set; }
        public string? Naam { get; set; }
        public string? Email { get; set; }
        public string? Postcode { get; set; }
        public string? Plaats { get; set; }
    }
}
