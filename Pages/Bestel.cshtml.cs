using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class BestelModel : PageModel
    {
        SqliteConnection connection;

        public string artikelNummer;
        public string artikelNaam;
        public double artikelPrijs;
        public string artikelAfbeelding;

        [Required, MaxLength(40, ErrorMessage = "Je mag maximaal 40 tekens hebben"), Display(Name = "naam")]
        public string naam { get; set; }

        [Required, Display(Name = "adres")]
        public string adres { get; set; }

        [Required, Display(Name = "woonplaats")]
        public string woonplaats { get; set; }

        public BestelModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "producten.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGet()
        {
            string? artikelnummer = Request.Query["artikelnummer"];

            if (artikelnummer != null)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM producten where id = {artikelnummer}";
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    artikelNummer = reader.GetInt32(0).ToString();  
                    artikelNaam = reader.GetString(1);             
                    artikelPrijs = reader.GetDouble(2);            
                    artikelAfbeelding = reader.GetString(3);       
                }
                connection.Close();
            }
            else
            {
                return RedirectToPage("Beheer");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO `bestellingen` (`naam`, `adres`, `woonplaats`, `bestelling`) VALUES ('{naam}', '{adres}', '{woonplaats}', '{artikelNummer}')";
            int reader = command.ExecuteNonQuery();
            connection.Close();

            return RedirectToPage("Index");
        }
    }
}
