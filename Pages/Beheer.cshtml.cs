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
    public class BeheerModel : PageModel
    {
        SqliteConnection connection;
        public List<string[]> shirts = new List<string[]>();

        public BeheerModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "producten.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Inlog");
        }


        public void OnGet()
        {
            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }

            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM producten";

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string[] shirt = new string[4];
                shirt[0] = reader.GetInt32(0).ToString();
                shirt[1] = reader.GetString(1);
                shirt[2] = reader.GetDouble(2).ToString();
                shirt[3] = reader.GetString(3);

                shirts.Add(shirt);
            }
            connection.Close();
        }
    }
}
