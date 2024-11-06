using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Entities;
using Web.Repositories;

namespace Web.Pages
{
    public class SkuModel(IPriceRepository repository) : PageModel
    {
        public string SKU { get; set; }
        public List<Price> Prices { get; set; }

        public async Task OnGetAsync(string sku)
        {
            SKU = sku;
            Prices = await repository.GetPricesPerMarket(SKU);
        }
    }
}
