using WebApplication1.Models;

namespace WebApplication1.DAL
{
    public interface IContext
    {
        void SaveExchangeRatesToFile(List<ExchangeRate> exchangeRates, string filePath);
        List<ExchangeRate> LoadExchangeRatesFromFile(string filePath);
    }
}
