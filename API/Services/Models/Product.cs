using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Services.Models
{
    public class Product
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        //NOTE: Quantity appears to allow for decimal places (inquire)
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }
}
