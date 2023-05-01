using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {

        private readonly IExchangeRates _exchangeRates;

        public IndexModel(IExchangeRates exchangeRates)
        {
            _exchangeRates = exchangeRates;
        }

        public List<ExchangeRate> ExchangeRates { get; private set; }

        public async Task OnGetAsync()
        {
            _exchangeRates.FetchExchangeRates(); //save data to file
            ExchangeRates = await _exchangeRates.GetExchangeRatesAsync(); //get the data from file
        }

    }
}