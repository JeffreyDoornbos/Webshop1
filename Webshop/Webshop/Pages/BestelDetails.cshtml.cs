using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

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

            using (var connection = new SqliteConnection("Data Source=producten.db"))
            {
                connection.Open();

                // Read the customers from the database
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM VW_BestellingenDetails where bestelID = " + BestelID.ToString();
                using (var reader = command.ExecuteReader())
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
