using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        //public void OnGet()
        //{

        //}

        private readonly IExchangeRates _exchangeRates;

        public IndexModel(IExchangeRates exchangeRates)
        {
            _exchangeRates = exchangeRates;
        }

        public List<ExchangeRate> ExchangeRates { get; private set; }

        public async Task OnGetAsync()
        {
            ExchangeRates = await _exchangeRates.GetExchangeRatesAsync();
        }

    }
}