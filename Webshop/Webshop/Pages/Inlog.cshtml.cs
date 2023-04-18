using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Webshop.Pages
{
    public class InlogModel : PageModel
    {

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

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM users WHERE username = @username AND password = @password";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@username";
            paramA.Value = Gebruikersnaam;
            dbCommand.Parameters.Add(paramA);

            SqlParameter paramN = new SqlParameter();
            paramN.ParameterName = "@password";
            paramN.Value = Wachtwoord;
            dbCommand.Parameters.Add(paramN);



                var reader = dbCommand.ExecuteReader();

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
