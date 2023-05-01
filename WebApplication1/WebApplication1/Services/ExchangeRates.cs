using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplication1.Models;
using WebApplication1.Services;

public class ExchangeRates : IExchangeRates
{
    //private readonly string ApiKey = Environment.GetEnvironmentVariable("OpenExchangeRatesApiKey");6c6de117ab74498caec1adb938e51e22
    private readonly string ApiKey = "6c6de117ab74498caec1adb938e51e22";
    private readonly string exchangeRatesFilePath = "C:\\Users\\Administrator\\OneDrive\\שולחן העבודה\\data.txt";
    private const string BaseUrl = "https://openexchangerates.org/api/latest.json";

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        // exchangeRates = new List<ExchangeRate>();

        var exchangeRates = LoadExchangeRatesFromFile(exchangeRatesFilePath);

        return exchangeRates;
    }

    public async void FetchExchangeRates()
    {

        var exchangeRates = new List<ExchangeRate>();

        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(BaseUrl);
            var response = await httpClient.GetAsync($"?app_id={ApiKey}&nocache=true");
            if (!response.IsSuccessStatusCode)
            {
                // return null;
                Console.WriteLine("failed to fetch data");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ExchangeRateData>(json);

            long unixTimestamp = data.date;
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp);
            string formattedDate = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine(data + json);
            var rates = new List<ExchangeRate>();

            // add the ExchangeRate objects to the list
            var usdIlsRate = new ExchangeRate
            {
                Name = "USD/ILS",
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(data.date),
                Rate = data.Rates["ILS"] / data.Rates["USD"]
            };

            var gbpEurRate = new ExchangeRate
            {
                Name = "GBP/EUR",
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(data.date),
                Rate = data.Rates["EUR"] / data.Rates["GBP"]
            };

            var eurJpyRate = new ExchangeRate
            {
                Name = "EUR/JPY",
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(data.date),
                Rate = data.Rates["JPY"] / data.Rates["EUR"]
            };

            var eurUsdRate = new ExchangeRate
            {
                Name = "EUR/USD",
                LastUpdated = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(data.date),
                Rate = data.Rates["USD"] / data.Rates["EUR"]
            };

            exchangeRates.Add(usdIlsRate);
            exchangeRates.Add(gbpEurRate);
            exchangeRates.Add(eurJpyRate);
            exchangeRates.Add(eurUsdRate);

            SaveExchangeRatesToFile(exchangeRates, exchangeRatesFilePath);

        }
    }

    private static void SaveExchangeRatesToFile(List<ExchangeRate> exchangeRates, string filePath)
    {
        using (var file = File.CreateText(filePath))
        {
            var serializer = new JsonSerializer();
            serializer.Serialize(file, exchangeRates);
        }
    }

    private static List<ExchangeRate> LoadExchangeRatesFromFile(string filePath)
    {
        using (var file = File.OpenText(filePath))
        {
            var serializer = new JsonSerializer();
            return (List<ExchangeRate>)serializer.Deserialize(file, typeof(List<ExchangeRate>));
        }
    }

    private class ExchangeRateData
    {
        public long date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}