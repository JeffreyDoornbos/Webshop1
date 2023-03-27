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
    public class insertModel : PageModel
    {
        [BindProperty, Required]
        public string Naam { get; set; }
        [BindProperty, Required]
        public decimal Prijs { get; set; }

        [BindProperty]
        public IFormFile Afbeelding { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Afbeelding == null || Afbeelding.Length == 0)
                return Page();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", Afbeelding.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Afbeelding.CopyToAsync(fileStream);
            }

            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=producten.db"))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO producten (naam, prijs, afbeelding) VALUES (@naam, @prijs, @afbeelding)";
                    command.Parameters.AddWithValue("@naam", Naam);
                    command.Parameters.AddWithValue("@prijs", Prijs);
                    command.Parameters.AddWithValue("@afbeelding", Afbeelding.FileName);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage("Beheer");
        }
    }
}
