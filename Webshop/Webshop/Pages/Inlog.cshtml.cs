using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class InlogModel : PageModel
    {
        private readonly string connectionString = "DataSource=users.db";

        [BindProperty]
        public string? Gebruikersnaam { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string? Wachtwoord { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Gebruikersnaam) || string.IsNullOrEmpty(Wachtwoord))
            {
                ModelState.AddModelError(string.Empty, "Gebruikersnaam en wachtwoord zijn verplicht.");
                return Page();
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM users WHERE username = @username AND password = @password";
                selectCommand.Parameters.AddWithValue("@username", Gebruikersnaam);
                selectCommand.Parameters.AddWithValue("@password", Wachtwoord);

                var reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetString("Username", Gebruikersnaam);
                    return RedirectToPage("/Beheer");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Gebruikersnaam of wachtwoord is onjuist.");
                    return Page();
                }
            }
        }
    }
}
