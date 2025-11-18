using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Client.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
        Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency);
        Task<List<HistoricalExchangeRate>> GetHistoricalRatesAsync(string fromCurrency, string toCurrency, int days);
        Task<List<Currency>> GetAvailableCurrenciesAsync();
        Task<List<Currency>> GetPopularCurrenciesAsync();
    }
}