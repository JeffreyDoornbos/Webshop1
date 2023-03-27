using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class UserManagementModel : PageModel
    {

        //public void OnPost()
        //{
        //    string? username = HttpContext.Session.GetString("Username");
        //    if (username == null)
        //    {
        //        // Redirect to login page if the user is not logged in
        //        Response.Redirect("/Inlog");
        //    }
        //}

        public class User
        {
            public int Id { get; set; }
            public string? Gebruikersnaam { get; set; }
            public string? Wachtwoord { get; set; }
            public string? Email { get; set; }
        }

        private readonly string connectionString = "DataSource=users.db";

        public List<User> Users { get; set; }



        public async Task OnGetAsync()
        {
            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM users";

                var reader = selectCommand.ExecuteReader();

                Users = new List<User>();

                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Gebruikersnaam = reader.GetString(1),
                        Wachtwoord = reader.GetString(2),
                        Email = reader.GetString(3)
                    };

                    Users.Add(user);
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = $"DELETE FROM users WHERE id = @id";
                deleteCommand.Parameters.AddWithValue("@id", id);

                deleteCommand.ExecuteNonQuery();

                return RedirectToPage("/UserManagement");
            }
        }
    }
}
