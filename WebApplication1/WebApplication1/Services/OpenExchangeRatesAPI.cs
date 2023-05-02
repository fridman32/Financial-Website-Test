using Newtonsoft.Json;
using System;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class OpenExchangeRatesAPI : IAPIInterface<OpenExchangeRatesAPI>
    {

        private readonly string ApiKey;
        private readonly string BaseUrl;
        private readonly IConfiguration _configuration;
        
        public OpenExchangeRatesAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration.GetValue<string>("OpenExchangeRatesApiKey");
            BaseUrl = _configuration.GetValue<string>("OpenExchangeRatesBaseUrl");
        }

        public async Task<List<ExchangeRate>> GetDataFromAPI()
        {
            using (var httpClient = new HttpClient())
            {
                //GET THE DATA FROM API
                httpClient.BaseAddress = new Uri(BaseUrl);
                var response = await httpClient.GetAsync($"?app_id={ApiKey}&nocache=true&");

                //VALIDATE SUCCESS
                if (!response.IsSuccessStatusCode)
                    return null;

                //Deserialize DATA
                var json = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<ExchangeRateData>(json);

                // add the ExchangeRate objects to the list
                List<ExchangeRate> rates = CreateExchangeRateList(data);
                return rates;
            }
        }


        private List<ExchangeRate> CreateExchangeRateList(ExchangeRateData data)
        {
            // SET THE DATA IN THE REQUESTED PAIRS
            var exchangeRates = new List<ExchangeRate>();

            var usdIlsRate = new ExchangeRate
            {
                Name = "USD/ILS",
                LastUpdated = DateTime.UtcNow,
                Rate = data.Rates["ILS"] / data.Rates["USD"]
            };

            var gbpEurRate = new ExchangeRate
            {
                Name = "GBP/EUR",
                LastUpdated = DateTime.UtcNow,
                Rate = data.Rates["EUR"] / data.Rates["GBP"]
            };

            var eurJpyRate = new ExchangeRate
            {
                Name = "EUR/JPY",
                LastUpdated = DateTime.UtcNow,
                Rate = data.Rates["JPY"] / data.Rates["EUR"]
            };

            var eurUsdRate = new ExchangeRate
            {
                Name = "EUR/USD",
                LastUpdated = DateTime.UtcNow,
                Rate = data.Rates["USD"] / data.Rates["EUR"]
            };

            exchangeRates.Add(usdIlsRate);
            exchangeRates.Add(gbpEurRate);
            exchangeRates.Add(eurJpyRate);
            exchangeRates.Add(eurUsdRate);

            return exchangeRates;
        }

    }
}
