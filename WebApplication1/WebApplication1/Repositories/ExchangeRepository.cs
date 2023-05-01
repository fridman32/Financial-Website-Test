using Microsoft.AspNetCore.DataProtection.KeyManagement;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class ExchangeRepository : IExchangeRepository
    {

        private readonly IContext _context;

        public ExchangeRepository(IContext context)
        {
            _context = context;
        }


        public void SaveData(List<ExchangeRate> exchangeRates, string filePath)
        {
            _context.SaveExchangeRatesToFile(exchangeRates, filePath);
        }
        public List<ExchangeRate> LoadData(string filePath)
        {
            var exchangeRates = _context.LoadExchangeRatesFromFile(filePath);
            return exchangeRates;
        }
    }
}
