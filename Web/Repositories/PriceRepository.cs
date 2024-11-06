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
    public class PriceRepository(AppDbContext context) : IPriceRepository
    {
        public Price Add(Price price)
        {
            return context.Prices.Add(price).Entity;
        }

        public async Task<int> GetTotalAsync()
        {
            var result = await context.Prices.OrderBy(x => x.Id)
                                             .CountAsync();
            return result;
        }

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

        public async Task<IList<Price>> GetPaginatedAsync(Paging pager)
        {
            var prices = await context.Prices
                                 .OrderBy(x => x.Id)
                                 .Skip(pager.Skip)
                                 .Take(pager.PageSize)
                                 .ToListAsync();
            return prices;
        }

        public async Task<IList<Price>> GetAllAsync()
        {
            var prices = await context.Prices
                                 .OrderBy(x => x.Id)
                                 .ToListAsync();
            return prices;
        }

        public async Task<Price> FindAsync(int id)
        {
            var price = await context.Prices
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            return price;
        }

        public async Task<List<Price>> GetPricesPerMarket(string sku)
        {
            List<Price> prices = [];

            // Gruppera efter MarketId och CurrencyCode för att
            // hantera varje marknad och valuta separat
            var results = await context.Prices.Where(x => x.SKU == sku)
                               .GroupBy(p => new { p.MarketId, p.CurrencyCode })
                               .ToListAsync();

            foreach (var group in results)
            {
                // Skapa unika tidsgränser för denna marknad och valuta
                var periods = new SortedSet<DateTime>();
                foreach (var price in group)
                {
                    periods.Add(price.ValidFrom);
                    if (price.ValidUntil.HasValue)
                    {
                        periods.Add(price.ValidUntil.Value);
                    }
                }

                // Iterera över delperioderna för att få lägsta pris
                var periodArray = periods.ToArray();
                for (int i = 0; i < periods.Count; i++)
                //for (int i = 0; i < periodArray.Length; i++)
                {
                    DateTime start = periods.ElementAtOrDefault(i);
                    //DateTime start = periodArray[i];
                    DateTime end = periodArray.ElementAtOrDefault(i + 1);

                    // Hämta det lägsta priset som är giltigt för denna delperiod
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
