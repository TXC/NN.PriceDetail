using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Entities;
using Web.Support;

namespace Web.Repositories
{
    /// <summary>
    /// Handle price table
    /// </summary>
    /// <param name="context"></param>
    public class PriceRepository(AppDbContext context) : IPriceRepository
    {
        /// <summary>
        /// Add new row
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public Price Add(Price price)
        {
            return context.Prices.Add(price).Entity;
        }

        /// <summary>
        /// Get total number of rows
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalAsync()
        {
            var result = await context.Prices.OrderBy(x => x.Id)
                                             .CountAsync();
            return result;
        }

        /// <summary>
        /// Get a limited resultset based on <paramref name="page"/> and <paramref name="size"/>
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<IList<Price>> GetPaginatedAsync(int page, int size)
        {
            var skip = (page - 1) * size;
            int count = await context.Prices
                                     .OrderBy(x => x.Id)
                                     .CountAsync();

            var pager = Paging.Create(count, page, size);

            var prices = await GetPaginatedAsync(pager);
            return prices;
        }

        /// <summary>
        /// Get a limited resultset based on <paramref name="pager"/>
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public async Task<IList<Price>> GetPaginatedAsync(Paging pager)
        {
            var prices = await context.Prices
                                 .OrderBy(x => x.Id)
                                 .Skip(pager.Skip)
                                 .Take(pager.PageSize)
                                 .ToListAsync();
            return prices;
        }

        /// <summary>
        /// Get all rows
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Price>> GetAllAsync()
        {
            var prices = await context.Prices
                                 .OrderBy(x => x.Id)
                                 .ToListAsync();
            return prices;
        }

        /// <summary>
        /// Locate specific row
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Price> FindAsync(int id)
        {
            var price = await context.Prices
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            return price;
        }

        /// <summary>
        /// Get an optimized resultset for <paramref name="sku"/>
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public async Task<List<Price>> GetPricesPerMarket(string sku)
        {
            List<Price> prices = [];

            // Group on MarketId and CurrencyCode to handle each
            // market and currency separatly
            var results = await context.Prices.Where(x => x.SKU == sku)
                               .GroupBy(p => new { p.MarketId, p.CurrencyCode })
                               .ToListAsync();

            foreach (var group in results)
            {
                // Create unique periods for this market and currency
                var periods = new SortedSet<DateTime>();
                foreach (var price in group)
                {
                    periods.Add(price.ValidFrom);
                    if (price.ValidUntil.HasValue)
                    {
                        periods.Add(price.ValidUntil.Value);
                    }
                }

                // Iterate over period to find the lowest price
                for (int i = 0; i < periods.Count; i++)
                {
                    DateTime start = periods.ElementAtOrDefault(i);
                    DateTime end = periods.ElementAtOrDefault(i + 1);

                    // Get the lowest price that is valid for this period
                    var validPrice = group.Where(p =>
                                            p.ValidFrom <= start
                                            && (!p.ValidUntil.HasValue
                                                || p.ValidUntil > start)
                                          )
                                          .OrderBy(p => p.UnitPrice)
                                          .FirstOrDefault();

                    if (validPrice is not null)
                    {
                        prices.Add(new Price()
                        {
                            MarketId = validPrice.MarketId,
                            UnitPrice = validPrice.UnitPrice,
                            CurrencyCode = validPrice.CurrencyCode,
                            ValidFrom = start,
                            ValidUntil = end != DateTime.MinValue ? end : null,
                        });
                    }
                }
            }
            return prices;
        }
    }
}
