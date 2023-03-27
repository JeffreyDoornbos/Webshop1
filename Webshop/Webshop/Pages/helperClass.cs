using System;
using static Webshop.Pages.CartModel;
using System.Text.Json;
using System.Collections.Generic;

namespace Webshop.Pages
{
	public class helperClass
	{
	

            // Define a method to calculate the total number of products
            public static int GetTotalProducts(Dictionary<string, int> cartItems)
            {
            
            int totalProducts = 0;
                foreach (var item in cartItems)
                {
                    totalProducts += item.Value;
                }
                return totalProducts;
            }
        
	}
}

