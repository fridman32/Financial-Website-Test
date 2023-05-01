namespace WebApplication1.Models
{
    public class ExchangeRate
    {
        public string Name { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public decimal Rate { get; set; }   
    }
}
