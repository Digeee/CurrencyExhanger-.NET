namespace CurrencyExchangeApp.Client.Models
{
    public class ExchangeRate
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}