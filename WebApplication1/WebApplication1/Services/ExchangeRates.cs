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
    private readonly string BackUpUrl;
    

    private readonly IExchangeRepository _exchangeRepository;
    private readonly IConfiguration _configuration;

    public ExchangeRates(IExchangeRepository exchangeRepository, IConfiguration configuration)
    {
        _exchangeRepository = exchangeRepository;
        _configuration = configuration;
        ApiKey = _configuration.GetValue<string>("OpenExchangeRatesApiKey");
        exchangeRatesFilePath = _configuration.GetValue<string>("exchangeRatesFilePath");
        BaseUrl = _configuration.GetValue<string>("BaseUrl");
        BackUpUrl = _configuration.GetValue<string>("BackUpUrl");   
    }

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        var exchangeRates = _exchangeRepository.LoadData(exchangeRatesFilePath);
        Console.WriteLine(exchangeRates);
        return exchangeRates;
    }

    public async Task FetchExchangeRates()
    {
        var response = GetDataFromAPI(BaseUrl + $"?app_id={ApiKey}&nocache=true", "");

        if (!response.Result.IsSuccessStatusCode)
        {
            response = GetDataFromAPI(BackUpUrl, "");
        }

        var json = await response.Result.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ExchangeRateData>(json);

        // add the ExchangeRate objects to the list
        List<ExchangeRate> rates = CreateExchangeRateList(data);

        _exchangeRepository.SaveData(rates, exchangeRatesFilePath);


    }

    private async Task<HttpResponseMessage> GetDataFromAPI(string url, string apiKey)
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url);
            return response;
        }
    }

    private List<ExchangeRate> CreateExchangeRateList(ExchangeRateData data)
    {
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