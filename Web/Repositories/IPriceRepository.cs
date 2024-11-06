using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Entities;
using Web.Support;

namespace Web.Repositories
{
    public interface IPriceRepository
    {
        Price Add(Price price);
        Task<int> GetTotalAsync();
        Task<IList<Price>> GetPaginatedAsync(int page, int size);
        Task<IList<Price>> GetPaginatedAsync(Paging pager);
        Task<IList<Price>> GetAllAsync();
        Task<Price> FindAsync(int id);
        Task<List<Price>> GetPricesPerMarket(string sku);
    }
}
