using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Client.Services
{
    public class MockCurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly List<Currency> _currencies;
        private readonly Random _random = new();

        public MockCurrencyExchangeService()
        {
            _currencies = new List<Currency>
            {
                new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", FlagIcon = "🇺🇸" },
                new Currency { Code = "EUR", Name = "Euro", Symbol = "€", FlagIcon = "🇪🇺" },
                new Currency { Code = "GBP", Name = "British Pound", Symbol = "£", FlagIcon = "🇬🇧" },
                new Currency { Code = "JPY", Name = "Japanese Yen", Symbol = "¥", FlagIcon = "🇯🇵" },
                new Currency { Code = "AUD", Name = "Australian Dollar", Symbol = "A$", FlagIcon = "🇦🇺" },
                new Currency { Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", FlagIcon = "🇨🇦" },
                new Currency { Code = "CHF", Name = "Swiss Franc", Symbol = "Fr", FlagIcon = "🇨🇭" },
                new Currency { Code = "CNY", Name = "Chinese Yuan", Symbol = "¥", FlagIcon = "🇨🇳" },
                new Currency { Code = "SEK", Name = "Swedish Krona", Symbol = "kr", FlagIcon = "🇸🇪" },
                new Currency { Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", FlagIcon = "🇳🇿" }
            };
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            await Task.Delay(300); // Simulate network delay
            var rate = await GetExchangeRateAsync(fromCurrency, toCurrency);
            return Math.Round(amount * rate.Rate, 2);
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            await Task.Delay(200); // Simulate network delay
            
            // Generate a realistic exchange rate
            var rate = 0.5m + (decimal)(_random.NextDouble() * 2);
            
            return new ExchangeRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = rate,
                Timestamp = DateTime.UtcNow
            };
        }

        public async Task<List<HistoricalExchangeRate>> GetHistoricalRatesAsync(string fromCurrency, string toCurrency, int days)
        {
            await Task.Delay(500); // Simulate network delay
            
            var rates = new List<HistoricalExchangeRate>();
            var baseRate = 0.8m + (decimal)(_random.NextDouble() * 0.8);
            
            for (int i = days; i >= 0; i--)
            {
                var fluctuation = (decimal)(_random.NextDouble() * 0.1) - 0.05m; // ±5% fluctuation
                rates.Add(new HistoricalExchangeRate
                {
                    Date = DateTime.UtcNow.AddDays(-i),
                    Rate = baseRate + fluctuation
                });
            }
            
            return rates;
        }

        public async Task<List<Currency>> GetAvailableCurrenciesAsync()
        {
            await Task.Delay(100); // Simulate network delay
            return _currencies;
        }

        public async Task<List<Currency>> GetPopularCurrenciesAsync()
        {
            await Task.Delay(100); // Simulate network delay
            return _currencies.Take(5).ToList();
        }
    }
}