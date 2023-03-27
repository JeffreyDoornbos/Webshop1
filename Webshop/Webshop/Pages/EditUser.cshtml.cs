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
using static Webshop.Pages.UserManagementModel;

namespace Webshop.Pages
{
    public class EditUserModel : PageModel
    {
        private readonly string connectionString = "DataSource=users.db";

        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM users WHERE id = @id";
                selectCommand.Parameters.AddWithValue("@id", id);

                var reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    User = new User
                    {
                        Id = reader.GetInt32(0),
                        Gebruikersnaam = reader.GetString(1),
                        Wachtwoord = reader.GetString(2),
                        Email = reader.GetString(3)
                    };
                }
                else
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string gebruikersnaam, string email)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = $"UPDATE users SET username = @username, email = @email WHERE id = @id";
                updateCommand.Parameters.AddWithValue("@username", gebruikersnaam);
                updateCommand.Parameters.AddWithValue("@email", email);
                updateCommand.Parameters.AddWithValue("@id", id);

                updateCommand.ExecuteNonQuery();

                return RedirectToPage("/UserManagement");
            }
        }
    }
}
