using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IExchangeRates
    {
        Task<List<ExchangeRate>> GetExchangeRatesAsync();
        void FetchExchangeRates();
    }
}
