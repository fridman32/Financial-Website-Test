using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IExchangeRepository
    {
        void SaveData(List<ExchangeRate> exchangeRates, string filePath);
        List<ExchangeRate> LoadData(string filePath);
    }
}
