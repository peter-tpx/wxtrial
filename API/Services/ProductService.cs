using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using API.Services.Models;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class ProductService: IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //Initial Draft 
        public async Task<List<Product>> GetProductsAsync(string url) //string url)
        {
            var httpClient = _httpClientFactory.CreateClient("productsClient");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return new List<Product>();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(responseStream);

            return products.ToList();
        }
    }
}
