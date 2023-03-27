using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Webshop.Pages
{
    public class IndexModel : PageModel
    {
        SqliteConnection connection;
        public List<string[]> laptopss = new List<string[]>();

        public IndexModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "producten.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public class CartModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }

        public void OnGet(string filterOption, double? minPrice, double? maxPrice)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            if (string.IsNullOrEmpty(filterOption))
            {
                command.CommandText = $"SELECT * FROM producten";
            }
            else
            {
                command.CommandText = $"SELECT * FROM producten WHERE naam LIKE '%{filterOption}%'";
            }
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                command.CommandText += $" AND price BETWEEN {minPrice.Value} AND {maxPrice.Value}";
            }
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string[] laptops = new string[4];
                laptops[0] = reader.GetInt32(0).ToString();
                laptops[1] = reader.GetString(1);
                laptops[2] = reader.GetDouble(2).ToString();
                laptops[3] = reader.GetString(3);

                laptopss.Add(laptops);
            }
            connection.Close();
        }

        public void OnPostAddToCart(string id)
        {
            //Haalt de data uit de shopping cart session
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();

            if (shoppingCart.ContainsKey(id))
            {
                shoppingCart[id] += 1; 
            }
            else
            {
                shoppingCart[id] = 1; 
            }

            //Slaat de dingetje op :)
            HttpContext.Session.Set("ShoppingCart", shoppingCart);

            Response.Redirect("/Index");
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
