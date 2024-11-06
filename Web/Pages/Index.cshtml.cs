using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Entities;
using Web.Repositories;
using Web.Support;

namespace Web.Pages
{
    public class IndexModel(IPriceRepository repository) : PageModel
    {
        public IList<Price> Prices { get; set; }
        public Paging Pager { get; set; }


        public async Task OnGetAsync(int? p)
        {
            int pageSize = 100;
            p ??= 1;

            int count = await repository.GetTotalAsync();
            Pager = Paging.Create(count, (int)p, pageSize);

            Prices = await repository.GetPaginatedAsync(Pager);
        }
    }
}
