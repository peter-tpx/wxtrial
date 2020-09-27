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
    public class ShopperHistoryService: IShopperHistoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ShopperHistoryService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        //Initial Draft 
        public async Task<List<ShopperHistory>> GetHistoryAsync(string url) //string url)
        {
            var httpClient = _httpClientFactory.CreateClient("productsClient");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            //TODO revert back to async after initial test
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return new List<ShopperHistory>();


            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<IEnumerable<ShopperHistory>>(responseStream);

            return products.ToList();
        }
    }
}
