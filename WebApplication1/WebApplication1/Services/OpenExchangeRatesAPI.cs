using System;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class OpenExchangeRatesAPI : IAPIInterface<OpenExchangeRatesAPI>
    {

        private readonly string ApiKey;
        private readonly string exchangeRatesFilePath;
        private readonly string BaseUrl;
        private readonly string BackUpUrl;
        private readonly IConfiguration _configuration;
        
        public OpenExchangeRatesAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration.GetValue<string>("OpenExchangeRatesApiKey");
            BaseUrl = _configuration.GetValue<string>("OpenExchangeRatesBaseUrl");
        }

        public async Task<HttpResponseMessage> GetDataFromAPI()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                var response = await httpClient.GetAsync($"?app_id={ApiKey}&nocache=true");
                return response;
            }
        }
    }
}
