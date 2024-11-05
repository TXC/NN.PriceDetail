using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Entities;

namespace Web.Pages
{
    public class SkuModel(AppDbContext ctx) : PageModel
    {
        public string SKU { get; set; }
        public List<Price> Prices { get; set; }

        async public Task OnGetAsync(string sku)
        {
            SKU = sku;
            Prices = await GetPricesPerMarket(SKU);
        }

        async Task<List<Price>> GetPricesPerMarket(string sku)
        {
            List<Price> prices = [];

            // Gruppera efter MarketId och CurrencyCode för att
            // hantera varje marknad och valuta separat
            var results = await ctx.Prices.Where(x => x.SKU == sku)
                                          .GroupBy(p => new { p.MarketId, p.CurrencyCode})
                                          .ToListAsync();

            foreach (var group in results)
            {
                // Skapa unika tidsgränser för denna marknad och valuta
                var periods = new SortedSet<DateTime>();
                foreach (var price in group)
                {
                    periods.Add(price.ValidFrom);
                    if (price.ValidUntil.HasValue)
                        periods.Add(price.ValidUntil.Value);
                }

                // Iterera över delperioderna för att få lägsta pris
                var periodArray = periods.ToArray();
                for (int i = 0; i < periodArray.Length; i++)
                {
                    DateTime start = periodArray[i];
                    DateTime end = periodArray.ElementAtOrDefault(i + 1);

                    // Hämta det lägsta priset som är giltigt för denna delperiod
                    var validPrice = group.Where(p => p.ValidFrom <= start
                                                         && (!p.ValidUntil.HasValue || p.ValidUntil > start))
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
