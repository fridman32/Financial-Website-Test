using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Services;

public class ExchangeRates : IExchangeRates
{
    //private readonly string ApiKey = Environment.GetEnvironmentVariable("OpenExchangeRatesApiKey");6c6de117ab74498caec1adb938e51e22
    private readonly string ApiKey = "6c6de117ab74498caec1adb938e51e22";
    private readonly string exchangeRatesFilePath = "C:\\Users\\Administrator\\OneDrive\\שולחן העבודה\\data.txt";
    private const string BaseUrl = "https://openexchangerates.org/api/latest.json";

    private readonly IContext _context;

    public ExchangeRates(IContext context)
    {
        _context = context;
    }

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        var exchangeRates = _context.LoadExchangeRatesFromFile(exchangeRatesFilePath);
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

            // add the ExchangeRate objects to the list
            List<ExchangeRate> rates = CreateExchangeRateList(data);

            _context.SaveExchangeRatesToFile(exchangeRates, exchangeRatesFilePath);

        }
    }

    private List<ExchangeRate> CreateExchangeRateList(dynamic data)
    {
        var exchangeRates = new List<ExchangeRate>();

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

        return exchangeRates;
    }

}