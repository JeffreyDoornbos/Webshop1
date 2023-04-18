using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Webshop.Pages
{
    public class BeheerModel : PageModel
    {
        public List<string[]> product = new List<string[]>();

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Inlog");
        }


        public void OnGet()
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

            string query = "SELECT * FROM producten";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                string[] producten = new string[4];
                producten[0] = reader.GetInt32(0).ToString();
                producten[1] = reader.GetString(1);
                producten[2] = reader.GetInt32(2).ToString();
                producten[3] = reader.GetString(3);

                product.Add(producten);
            }
            dbConnection.Close();
        }
    }
}
