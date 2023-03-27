using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

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
            KlantBestellingen = new List<KlantBestellingen>();

            this.KlantID = KlantID;

            using (var connection = new SqliteConnection("Data Source=producten.db"))
            {
                connection.Open();

                // Read the customers from the database
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Vw_Bestellingen where klantID = "+ KlantID.ToString();
                using (var reader = command.ExecuteReader())
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
            }
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
