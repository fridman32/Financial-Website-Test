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

public class ExchangeRatesService : IExchangeRates
{
    private readonly string exchangeRatesFilePath;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IConfiguration _configuration;
    private readonly IAPIInterface<XeCurrencyDataAPI> _xeCurrencyDataAPI;
    private readonly IAPIInterface<OpenExchangeRatesAPI> _openExchangeRatesAPI;


    public ExchangeRatesService(IExchangeRepository exchangeRepository, IConfiguration configuration, IAPIInterface<XeCurrencyDataAPI> xeCurrencyDataApi, IAPIInterface<OpenExchangeRatesAPI> openExchangeRatesApi)
    {
        _exchangeRepository = exchangeRepository;
        _configuration = configuration;
        exchangeRatesFilePath = _configuration.GetValue<string>("exchangeRatesFilePath");
        _xeCurrencyDataAPI = xeCurrencyDataApi;
        _openExchangeRatesAPI = openExchangeRatesApi;
    }

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        //GET DATA FROM FILE
        var exchangeRates = _exchangeRepository.LoadData(exchangeRatesFilePath);
        return exchangeRates;
    }

    public async Task FetchExchangeRates()
    {
        //FETCH DATA FROM MAIN API
        var response = _openExchangeRatesAPI.GetDataFromAPI().Result;

        if (response == null) 
            response = _xeCurrencyDataAPI.GetDataFromAPI().Result; //USE BACKUP API
        //SAVE THE DATA TO FILE
        _exchangeRepository.SaveData(response, exchangeRatesFilePath);
    }

}