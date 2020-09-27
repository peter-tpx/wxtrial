using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Services.Models
{
    public class ShopperHistory
    {
        [JsonPropertyName("customerId")]
        public long CustomerId { get; set; }

        [JsonPropertyName("products")]
        public List<Product> Products { get; set; }
    }
}
