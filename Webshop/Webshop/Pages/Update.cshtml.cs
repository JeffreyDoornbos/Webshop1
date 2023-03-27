using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class UpdateModel : PageModel
    {
        public string? artikelnummer { get; set; }
        public string? naam { get; set; }
        public string? prijs { get; set; }
        public string? afbeelding { get; set; }

        public void OnGet(string artikelnummer)
        {
            this.artikelnummer = artikelnummer;

            string connectionString = "Data Source=producten.db";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM producten WHERE ID = @artikelnummer";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@artikelnummer", artikelnummer);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.naam = reader["naam"].ToString();
                            this.prijs = reader["prijs"].ToString();
                            this.afbeelding = reader["afbeelding"].ToString();
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {

            artikelnummer = Request.Form["artikelnummer"];
            naam = Request.Form["naam"];
            prijs = Request.Form["prijs"];
            afbeelding = Request.Form["afbeelding"];
            string connectionString = "Data Source=producten.db";


            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE producten SET naam = @naam, prijs = @prijs, afbeelding = @afbeelding WHERE ID = @artikelnummer";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@artikelnummer", artikelnummer);
                    command.Parameters.AddWithValue("@naam", naam);
                    command.Parameters.AddWithValue("@prijs", prijs);
                    command.Parameters.AddWithValue("@afbeelding", afbeelding);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Beheer");
        }
    }

}
