using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class AddUserModel : PageModel
    {
        private readonly string connectionString = "DataSource=users.db";


        [BindProperty]
        [Required]
        [Display(Name = "Gebruikersnaam")]
        public string? Gebruikersnaam { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string? Wachtwoord { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public void OnGet()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }

        }

            public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = $"INSERT INTO users (username, password, email) VALUES (@username, @password, @email)";
                insertCommand.Parameters.AddWithValue("@username", Gebruikersnaam);
                insertCommand.Parameters.AddWithValue("@password", Wachtwoord);
                insertCommand.Parameters.AddWithValue("@email", Email);

                insertCommand.ExecuteNonQuery();

                return RedirectToPage("/Beheer");
            }
        }
    }
}
