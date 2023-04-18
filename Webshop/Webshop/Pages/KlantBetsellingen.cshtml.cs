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
	public class KlantBetsellingenModel : PageModel
    {
        public int? KlantID { get; set; }
        public int? BestelID { get; set; }
        public string? naam { get; set; }
        public string? datum { get; set; }
        public string? betaalwijzen { get; set; }

        public List<KlantBestellingen> KlantBestellingen { get; set; }


        public void OnGet(int KlantID)
        {

            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
            KlantBestellingen = new List<KlantBestellingen>();

            this.KlantID = KlantID;

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM Vw_Bestellingen where klantID = " + KlantID.ToString();
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();


    
                using (var reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bestellingen = new KlantBestellingen
                        {
                            BestelID = reader.GetInt32(1),
                            naam = reader.GetString(4),
                            datum = reader.GetString(2),
                            betaalwijzen = reader.GetString(5)
                        };
                        KlantBestellingen.Add(bestellingen);
                    }
                }
            dbConnection.Close();
        }
    }

    public class KlantBestellingen
    {
        public int BestelID { get; set; }
        public string? datum { get; set; }
        public string? naam { get; set; }
        public string? betaalwijzen { get; set; }
    }
}
