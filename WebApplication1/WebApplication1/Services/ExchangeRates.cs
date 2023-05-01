using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplication1.Models;
using WebApplication1.Services;

public class ExchangeRates : IExchangeRates
{
    //private readonly string ApiKey = Environment.GetEnvironmentVariable("OpenExchangeRatesApiKey");6c6de117ab74498caec1adb938e51e22
    private readonly string ApiKey = "6c6de117ab74498caec1adb938e51e22";

    private const string BaseUrl = "https://openexchangerates.org/api/latest.json";

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        var exchangeRates = new List<ExchangeRate>();

        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(BaseUrl);
            var response = await httpClient.GetAsync($"?app_id={ApiKey}&symbols=USD,EUR,GBP");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ExchangeRateData>(json);

            Console.WriteLine(json);
            var rates = new List<ExchangeRate>();

            // create ExchangeRate objects for each currency pair
            var usdIlsRate = new ExchangeRate { FromCurrency = "USD", ToCurrency = "ILS", Rate = data.Rates["ILS"] / data.Rates["USD"] };
            var gbpEurRate = new ExchangeRate { FromCurrency = "GBP", ToCurrency = "EUR", Rate = data.Rates["EUR"] / data.Rates["GBP"] };
            var eurJpyRate = new ExchangeRate { FromCurrency = "EUR", ToCurrency = "JPY", Rate = data.Rates["JPY"] / data.Rates["EUR"] };

            // add the ExchangeRate objects to the list
            exchangeRates.Add(usdIlsRate);
            exchangeRates.Add(gbpEurRate);
            exchangeRates.Add(eurJpyRate);
            return rates;
        }

    }

    private class ExchangeRateData
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
}