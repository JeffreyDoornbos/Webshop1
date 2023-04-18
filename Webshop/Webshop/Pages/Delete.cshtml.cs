using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Webshop.Pages
{
    public class DeleteModel : PageModel
    {
        public int Artikelnummer { get; set; }

        public async Task<IActionResult> OnGetAsync(int artikelnummer)
        {
            Artikelnummer = artikelnummer;
            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "DELETE FROM producten WHERE ID = @artikelnummer";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@artikelnummer";
            paramA.Value = artikelnummer;
            dbCommand.Parameters.Add(paramA);

            dbCommand.ExecuteNonQuery();
            dbConnection.Close();

            return RedirectToPage("Beheer");
        }
    }
}
