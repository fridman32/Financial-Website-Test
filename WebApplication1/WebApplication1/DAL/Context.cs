using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.DAL
{
    public class Context : IContext
    {
        public void SaveExchangeRatesToFile(List<ExchangeRate> exchangeRates, string filePath)
        {
            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, exchangeRates);
            }
        }

        public List<ExchangeRate> LoadExchangeRatesFromFile(string filePath)
        {
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                return (List<ExchangeRate>)serializer.Deserialize(file, typeof(List<ExchangeRate>));
            }
        }
    }
}
