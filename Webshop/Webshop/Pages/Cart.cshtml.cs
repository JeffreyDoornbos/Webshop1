using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Webshop.Pages
{
    public class CartModel : PageModel
    {
        public List<CartItem> CartItems { get; set; }
        SqliteConnection connection;

        public class CartItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }

        public CartModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "producten.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public void OnGet()
        {
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();

            CartItems = new List<CartItem>();

            foreach (var item in shoppingCart)
            {
                string productId = item.Key;
                int quantity = item.Value;

                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM producten WHERE id = @productId";
                command.Parameters.AddWithValue("@productId", productId);
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    CartItems.Add(new CartItem
                    {
                        Id = reader.GetInt32(0).ToString(),
                        Name = reader.GetString(1),
                        Price = reader.GetDouble(2),
                        Quantity = quantity
                    });
                }

                connection.Close();
            }
        }

        public IActionResult OnPostEmptyCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToPage("/Cart");
        }
    }
}
