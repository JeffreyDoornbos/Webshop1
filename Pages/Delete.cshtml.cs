using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Webshop.Pages
{
    public class DeleteModel : PageModel
    {
        public int Artikelnummer { get; set; }

        public async Task<IActionResult> OnGetAsync(int artikelnummer)
        {
            Artikelnummer = artikelnummer;

            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=producten.db"))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM producten WHERE ID = @artikelnummer";
                    command.Parameters.AddWithValue("@artikelnummer", Artikelnummer);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("Beheer");
        }
    }
}
