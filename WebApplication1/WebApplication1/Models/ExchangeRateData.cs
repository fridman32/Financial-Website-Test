namespace WebApplication1.Models
{
    public class ExchangeRateData
    {
        public long date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
