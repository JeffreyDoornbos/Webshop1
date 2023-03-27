using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop.Pages
{
    public class KlantenSysteemModel : PageModel
    {
        public List<Klant> Klanten { get; set; }

        public void OnGet()
        {
            Klanten = new List<Klant>();

            // Connect to the database
            using (var connection = new SqliteConnection("Data Source=producten.db"))
            {
                connection.Open();

                // Read the customers from the database
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM klanten";
                using (var reader = command.ExecuteReader())
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
