namespace WebApplication1.Models
{
    public class XeCurrencyModel
    {
        public ToCurrency[] To { get; set; }
    }

    public class ToCurrency
    {
        public string currency { get; set; }
        public decimal mid { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public decimal timestamp { get; set; }
    }
}   
