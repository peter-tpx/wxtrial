using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Models.API
{
    public class OrderProduct
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    public class OrderSpecialQuantities
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }


    public class OrderSpecials
    {
        [JsonPropertyName("quantities")]
        public List<OrderSpecialQuantities> Quantities { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }

    public class OrderQuantities
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }


    public class TrolleyManifest
    {
        [JsonPropertyName("products")]
        public List<OrderProduct> Products { get; set; }

        [JsonPropertyName("specials")]
        public List<OrderSpecials> Specials { get; set; }

        [JsonPropertyName("quantities")]
        public List<OrderQuantities> Quantities { get; set; }
    }
}
