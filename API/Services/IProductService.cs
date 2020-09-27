using System.Collections.Generic;
using System.Threading.Tasks;
using API.Services.Models;

namespace API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync(string url);
    }
}