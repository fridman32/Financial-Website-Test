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
using WebApplication1.Repositories;
using WebApplication1.Services;

public class ExchangeRates : IExchangeRates
{
    private readonly string ApiKey;
    private readonly string exchangeRatesFilePath;
    private readonly string BaseUrl;

    private readonly IExchangeRepository _exchangeRepository;
    private readonly IConfiguration _configuration;

    public ExchangeRates(IExchangeRepository exchangeRepository, IConfiguration configuration)
    {
        _exchangeRepository = exchangeRepository;
        _configuration = configuration;
        ApiKey = _configuration.GetValue<string>("OpenExchangeRatesApiKey");
        exchangeRatesFilePath = _configuration.GetValue<string>("exchangeRatesFilePath");
        BaseUrl = _configuration.GetValue<string>("BaseUrl");
    }

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        var exchangeRates = _exchangeRepository.LoadData(exchangeRatesFilePath);
        Console.WriteLine(exchangeRates);
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

            _exchangeRepository.SaveData(exchangeRates, exchangeRatesFilePath);

        }
    }

    private List<ExchangeRate> CreateExchangeRateList(ExchangeRateData data)
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