using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Webshop.Pages
{
	public class BestelDetailsModel : PageModel
    {
        public int BestelID { get; set; }
        public int ProductID { get; set; }
        public string? naam { get; set; }
        public int Aantal { get; set; }
        public int totaalprijs { get; set; }
        public string? afbeelding { get; set; }

        public List<BestelDetails> BestelDetails { get; set; }



        public void OnGet(int BestelID)
        {
            BestelDetails = new List<BestelDetails>();

            this.BestelID = BestelID;

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM VW_BestellingenDetails where bestelID = " + BestelID.ToString();
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();


                using (var reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producten = new BestelDetails
                        {
                            BestelID = reader.GetInt32(0),
                            ProductID = reader.GetInt32(1),
                            naam = reader.GetString(2),
                            Aantal = reader.GetInt32(3),
                            totaalprijs = reader.GetInt32(4),
                            afbeelding = reader.GetString(5)
                        };
                        BestelDetails.Add(producten);
                    }
                }
        }
    }

    public class BestelDetails
    {
        public int BestelID { get; set; }
        public int ProductID { get; set; }
        public string? naam { get; set; }
        public int Aantal { get; set; }
        public int totaalprijs { get; set; }
        public string? afbeelding { get; set; }

    }
}
