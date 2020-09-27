using System.Text.Json.Serialization;

namespace API.Models.API
{
    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
        
        public decimal Quantity { get; set; }
    }
}
