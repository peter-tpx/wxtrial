using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Services;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceModels = API.Services.Models;
using APIModels = API.Models.API;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        //NOTE: Logger injection is supported although no specific logging has been implemented (yet) 
        private readonly ILogger<AnswersController> _logger;

        private readonly IProductService _productService;

        private readonly IShopperHistoryService _shopperHistory;

        public AnswersController(IConfiguration configuration, ILogger<AnswersController> logger, IProductService productService, IShopperHistoryService shopperHistory )
        {
            _logger = logger;
            _configuration = configuration;
            _productService = productService;
            _shopperHistory = shopperHistory;
        }

        [HttpGet]
        [Route("user")]
        public IActionResult GetUser()
        {
            var result = new
            {
                name = _configuration["Identifiers:name"],
                token = _configuration["Identifiers:token"]
            };

            return Ok(result);

        }


        [HttpGet]
        [Route("sort")]
        public async Task<IActionResult> GetSortedProductListAsync(string sortOption)
        {
            if (string.IsNullOrEmpty(sortOption))
            {
                return BadRequest("Api parameter 'sortOption' is missing");
            }

            var productUrl = BuildUrlWithToken(_configuration["ExternalServices:Products"],_configuration["Identifiers:token"]);

            var products = await _productService.GetProductsAsync(productUrl);

            
            List<ServiceModels.Product> sorted = new List<ServiceModels.Product>();

            if (sortOption.Equals("Low", StringComparison.CurrentCulture))
            {
                sorted = products.OrderBy(o => o.Price).ToList();
            }
            else if (sortOption.Equals("High", StringComparison.CurrentCulture))
            {
                sorted = products.OrderByDescending(o => o.Price).ToList();
            }
            else if (sortOption.Equals("Ascending", StringComparison.CurrentCulture))
            {
                sorted = products.OrderBy(o => o.Name).ToList();
            }
            else if (sortOption.Equals("Descending", StringComparison.CurrentCulture))
            {
                sorted = products.OrderByDescending(o => o.Name).ToList();
            }
            else if (sortOption.Equals("Recommended", StringComparison.CurrentCulture))
            {
                var historyUrl = BuildUrlWithToken(_configuration["ExternalServices:ShopperHistory"], _configuration["Identifiers:token"]);

                var history = _shopperHistory.GetHistoryAsync(historyUrl).Result.ToList();

                var mostPurchased = new List<ServiceModels.Product>();

                // build a list of purchases across customers 
                foreach (var h in history)
                {
                    if (h.Products != null)
                    {
                        foreach (var p in h.Products)
                        {
                            var idx = mostPurchased.FindIndex(x => x.Name.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase));
                            if (idx >= 0)
                            {
                                mostPurchased[idx].Quantity += p.Quantity;
                            }
                            else
                            {
                                mostPurchased.Add(p);
                            }
                        }
                    }
                }

                sorted = new List<ServiceModels.Product>();

                var notListed = new List<ServiceModels.Product>();

                foreach (var mp in mostPurchased.OrderByDescending(o => o.Quantity))
                {
                    if (products.ToList().Exists(p => p.Name.Equals(mp.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        sorted.Add(mp);
                    }
                    else
                    {
                        notListed.Add(mp);
                    }
                }

                // -- add non-listed products in history to the end 
                sorted.AddRange(notListed.OrderByDescending(o => o.Quantity));
            }
            else
            {
                return BadRequest("Api parameter 'sortOption' does not comply with the specification");
            }

            var mapped = ProductMapper.Map(sorted);

            return Ok(mapped);
        }

        private string BuildUrlWithToken(string template, string token)
        {
            return template.Replace("[TOKEN]", token);
        }

    }
}
