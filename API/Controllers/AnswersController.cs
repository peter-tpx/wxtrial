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


                var popularList =  mostPurchased.OrderByDescending(o => o.Quantity).ToList().ConvertAll(x => x.Name);

                sorted = products.OrderBy(x =>
                {
                    var index = Array.IndexOf(popularList.ToArray(), x.Name);
                    return index < 0 ? int.MaxValue : index;
                }).ToList();

            }
            else
            {
                return BadRequest("Api parameter 'sortOption' does not comply with the specification");
            }

            var mapped = ProductMapper.Map(sorted);

            return Ok(mapped);
        }


        [HttpPost]
        [Route("trolleyTotal")]
        public ActionResult<decimal> GetTrollyTotalAsync(APIModels.TrolleyManifest manifest)
        {
            //TODO refactor - add validation to the model 
            if (!ModelState.IsValid)
            {
                return BadRequest("Api parameter 'manifest' is missing or is not valid");
            }

            // Assumption:  Manifest may include only one quantity item match per product name
            var purchased = (from prod in manifest.Products
                let qty = manifest.Quantities.Find(x => x.Name.Equals(prod.Name, StringComparison.CurrentCultureIgnoreCase))
                    ?.Quantity ?? 0
                select new ServiceModels.Product() {Name = prod.Name, Quantity = qty, Price = prod.Price}).ToList();

            var total = (from p in purchased
                let itemTotal = p.Quantity * p.Price
                let specials = manifest.Specials.Where(x => x.Quantities.Any(y => y.Name.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase)))
                select specials.Where(s => s.Quantities.Any(x => x.Quantity <= p.Quantity))
                    .Aggregate(itemTotal, (current, s) => current - s.Total)).Sum();


            return Ok(total);
        }

        private string BuildUrlWithToken(string template, string token)
        {
            return template.Replace("[TOKEN]", token);
        }

    }
}
