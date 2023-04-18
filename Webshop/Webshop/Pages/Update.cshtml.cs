using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;



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
            string? username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("/Inlog");
            }
            this.artikelnummer = artikelnummer;

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "SELECT * FROM producten WHERE ID = @artikelnummer";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@artikelnummer";
            param.Value = artikelnummer;
            dbCommand.Parameters.Add(param);
            using (var reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    this.naam = reader["naam"].ToString();
                    this.prijs = reader["prijs"].ToString();
                    this.afbeelding = reader["afbeelding"].ToString();
                }
            }
            dbConnection.Close();
           
        }

        public IActionResult OnPost()
        {

            artikelnummer = Request.Form["artikelnummer"];
            naam = Request.Form["naam"];
            prijs = Request.Form["prijs"];
            afbeelding = Request.Form["afbeelding"];


            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);
            string query = "UPDATE producten SET naam = @naam, prijs = @prijs, afbeelding = @afbeelding WHERE ID = @artikelnummer";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            SqlParameter paramA = new SqlParameter();
            paramA.ParameterName = "@artikelnummer";
            paramA.Value = artikelnummer;
            dbCommand.Parameters.Add(paramA);

            SqlParameter paramN = new SqlParameter();
            paramN.ParameterName = "@naam";
            paramN.Value = naam;
            dbCommand.Parameters.Add(paramN);

            SqlParameter paramP = new SqlParameter();
            paramP.ParameterName = "@prijs";
            paramP.Value = prijs;
            dbCommand.Parameters.Add(paramP);

            SqlParameter paramAfb = new SqlParameter();
            paramAfb.ParameterName = "@afbeelding";
            paramAfb.Value = afbeelding;
            dbCommand.Parameters.Add(paramAfb);

            dbCommand.ExecuteNonQuery();
            dbConnection.Close();
            return RedirectToPage("/Beheer");
        }
    }
}

