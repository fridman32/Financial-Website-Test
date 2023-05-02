namespace WebApplication1.Services
{
    public class XeCurrencyDataAPI : IAPIInterface<XeCurrencyDataAPI>
    {

        private readonly string ApiKey;
        private readonly string exchangeRatesFilePath;
        private readonly string BaseUrl;
        private readonly string BackUpUrl;
        private readonly IConfiguration _configuration;

        public XeCurrencyDataAPI(IConfiguration configuration)
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
