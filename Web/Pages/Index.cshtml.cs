using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Entities;

namespace Web.Pages
{
    public class IndexModel(AppDbContext ctx) : PageModel
    {
        public IList<Price> Prices { get; set; }
        public Paging Pager { get; set; }


        public async Task OnGetAsync(int? p)
        {
            int pageSize = 100;
            p ??= 1;

            var skip = (p - 1) * pageSize;

            var PriceQuery = ctx.Prices.OrderBy(x => x.Id);
            int count = await PriceQuery.CountAsync();

            Pager = Paging.Create(count, (int)p, pageSize);


            Prices = await PriceQuery.Skip(Pager.Skip)
                                     .Take(Pager.PageSize)
                                     .ToListAsync();
        }
    }
}
