using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceModels = API.Services.Models;
using APIModels = API.Models.API;

namespace API.Helpers
{
    public class ProductMapper
    {
        //NOTE: This helper class is created just an interim solution. AutoMapper will be more appropriate for API with multiple models

        public static List<APIModels.Product> Map(List<ServiceModels.Product> products)
        {
            if (products == null)
            {
                return new List<APIModels.Product>();
            }
            
            return products.ConvertAll(x => new APIModels.Product
            {
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity
            });
        }

    }
}
