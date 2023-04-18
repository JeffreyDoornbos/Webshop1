using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Webshop.Pages
{
    public class CartModel : PageModel
    {
        public List<CartItem> CartItems { get; set; }


        public class CartItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }


        public void OnGet()
        {
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();

            CartItems = new List<CartItem>();

            foreach (var item in shoppingCart)
            {
                string productId = item.Key;
                int quantity = item.Value;

                // Connect to the database
                string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
                IDbConnection dbConnection = new SqlConnection(Conn);

                IDbCommand dbCommand = new SqlCommand();
                dbCommand.Connection = dbConnection;
                dbConnection.Open();

                dbCommand.CommandText = $"SELECT * FROM producten WHERE id = @productId";

                SqlParameter paramA = new SqlParameter();
                paramA.ParameterName = "@productId";
                paramA.Value = productId;
                dbCommand.Parameters.Add(paramA);

                var reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    CartItems.Add(new CartItem
                    {
                        Id = reader.GetInt32(0).ToString(),
                        Name = reader.GetString(1),
                        Price = reader.GetInt32(2),
                        Quantity = quantity
                    });
                }

                dbConnection.Close();
            }
        }

        public IActionResult OnPostEmptyCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToPage("/Cart");
        }
    }
}
