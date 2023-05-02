using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class XeCurrencyDataAPI : IAPIInterface<XeCurrencyDataAPI>
    {

        private readonly string ApiKey;
        private readonly string AccountId;
        private readonly string BaseUrl;
        private readonly IConfiguration _configuration;

        public XeCurrencyDataAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration.GetValue<string>("XeCurrencyDataAPIApiKey");
            AccountId = _configuration.GetValue<string>("XeCurrencyDataAPIApiAccountId");
            BaseUrl = _configuration.GetValue<string>("XeCurrencyDataAPIBaseUrl");
        }

        public async Task<List<ExchangeRate>> GetDataFromAPI()
        {
            var exchangeRates = new List<ExchangeRate>();
            //SET THE PAIRS

            // USD to EUR
            var usdEurRate = await GetExchangeRate("USD", "ILS");
            exchangeRates.Add(new ExchangeRate
            {
                Name = "USD/ILS",
                LastUpdated = DateTime.UtcNow,
                Rate = usdEurRate
            });

            // USD to JPY
            var usdJpyRate = await GetExchangeRate("GBP", "EUR");
            exchangeRates.Add(new ExchangeRate
            {
                Name = "GBP/EUR",
                LastUpdated = DateTime.UtcNow,
                Rate = usdJpyRate
            });

            // USD to GBP
            var usdGbpRate = await GetExchangeRate("EUR", "USD");
            exchangeRates.Add(new ExchangeRate
            {
                Name = "EUR/USD",
                LastUpdated = DateTime.UtcNow,
                Rate = usdGbpRate
            });

            // EUR to JPY
            var eurJpyRate = await GetExchangeRate("EUR", "JPY");
            exchangeRates.Add(new ExchangeRate
            {
                Name = "EUR/JPY",
                LastUpdated = DateTime.UtcNow,
                Rate = eurJpyRate
            });

            return exchangeRates;
        }

        private async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
        {
            //GET DATA FOR PAIR
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(AccountId + ":" + ApiKey)));
                var response = await httpClient.GetAsync($"convert_from.json/?from={fromCurrency}&to={toCurrency}&amount=1");
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<XeCurrencyModel>(json);
                return data.To[0].mid;
            }
        }

    }
}
