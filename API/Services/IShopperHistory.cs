using System.Collections.Generic;
using System.Threading.Tasks;
using API.Services.Models;

namespace API.Services
{
    public interface IShopperHistoryService
    {
        Task<List<ShopperHistory>> GetHistoryAsync(string url);
    }
}