using System.Net.Http.Json;
using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Client.Services
{
    public class ExchangeRateApiService : ICurrencyExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.exchangerate-api.com/v4/latest/";

        public ExchangeRateApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Currency>> GetAvailableCurrenciesAsync()
        {
            // First try to get from our internal API
            try
            {
                var response = await _httpClient.GetAsync("api/marketdata/currencies");
                if (response.IsSuccessStatusCode)
                {
                    var currencies = await response.Content.ReadFromJsonAsync<IEnumerable<Currency>>();
                    return currencies?.ToList() ?? new List<Currency>();
                }
            }
            catch
            {
                // If our internal API fails, use fallback data
            }

            // Fallback to static data
            return new List<Currency>
            {
                new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", FlagIcon = "🇺🇸" },
                new Currency { Code = "EUR", Name = "Euro", Symbol = "€", FlagIcon = "🇪🇺" },
                new Currency { Code = "GBP", Name = "British Pound", Symbol = "£", FlagIcon = "🇬🇧" },
                new Currency { Code = "JPY", Name = "Japanese Yen", Symbol = "¥", FlagIcon = "🇯🇵" },
                new Currency { Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", FlagIcon = "🇨🇦" },
                new Currency { Code = "AUD", Name = "Australian Dollar", Symbol = "A$", FlagIcon = "🇦🇺" },
                new Currency { Code = "CHF", Name = "Swiss Franc", Symbol = "Fr", FlagIcon = "🇨🇭" },
                new Currency { Code = "CNY", Name = "Chinese Yuan", Symbol = "¥", FlagIcon = "🇨🇳" },
                new Currency { Code = "SEK", Name = "Swedish Krona", Symbol = "kr", FlagIcon = "🇸🇪" },
                new Currency { Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", FlagIcon = "🇳🇿" },
                new Currency { Code = "LKR", Name = "Sri Lankan Rupee", Symbol = "Rs", FlagIcon = "🇱🇰" }
            };
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            // First try to get from our internal API
            try
            {
                var response = await _httpClient.GetAsync($"api/marketdata/rates/{fromCurrency}/{toCurrency}");
                if (response.IsSuccessStatusCode)
                {
                    var exchangeRate = await response.Content.ReadFromJsonAsync<ExchangeRate>();
                    return exchangeRate ?? new ExchangeRate { FromCurrency = fromCurrency, ToCurrency = toCurrency, Rate = 1m, Timestamp = DateTime.UtcNow };
                }
            }
            catch
            {
                // If our internal API fails, use external API
            }

            // Fallback to external API
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(_baseUrl + fromCurrency);
                if (response?.Rates?.ContainsKey(toCurrency) == true)
                {
                    return new ExchangeRate 
                    { 
                        FromCurrency = fromCurrency, 
                        ToCurrency = toCurrency, 
                        Rate = (decimal)response.Rates[toCurrency], 
                        Timestamp = response.Date 
                    };
                }
            }
            catch
            {
                // If external API fails, return default rate
            }

            return new ExchangeRate { FromCurrency = fromCurrency, ToCurrency = toCurrency, Rate = 1m, Timestamp = DateTime.UtcNow }; // Default rate
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            // First try to get from our internal API
            try
            {
                var response = await _httpClient.GetAsync($"api/marketdata/convert/{fromCurrency}/{toCurrency}/{amount}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<decimal>();
                    return result;
                }
            }
            catch
            {
                // If our internal API fails, calculate using exchange rate
            }

            // Fallback to calculating using exchange rate
            var exchangeRate = await GetExchangeRateAsync(fromCurrency, toCurrency);
            return amount * exchangeRate.Rate;
        }

        public async Task<List<HistoricalExchangeRate>> GetHistoricalRatesAsync(string fromCurrency, string toCurrency, int days = 30)
        {
            // First try to get from our internal API
            try
            {
                var response = await _httpClient.GetAsync($"api/marketdata/historical/{fromCurrency}/{toCurrency}");
                if (response.IsSuccessStatusCode)
                {
                    var historicalRates = await response.Content.ReadFromJsonAsync<IEnumerable<HistoricalExchangeRate>>();
                    return historicalRates?.ToList() ?? new List<HistoricalExchangeRate>();
                }
            }
            catch
            {
                // If our internal API fails, use fallback data
            }

            // Fallback to simulated data
            var rates = new List<HistoricalExchangeRate>();
            var random = new Random();
            var baseRate = 0.8 + random.NextDouble() * 1.5; // Random base rate between 0.8 and 2.3

            for (int i = days; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                var rate = baseRate + (random.NextDouble() - 0.5) * 0.1; // Small variation
                
                rates.Add(new HistoricalExchangeRate
                {
                    Date = date,
                    Rate = (decimal)rate
                });
            }

            return rates;
        }

        public async Task<List<Currency>> GetPopularCurrenciesAsync()
        {
            // Return popular currencies
            return new List<Currency>
            {
                new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", FlagIcon = "🇺🇸" },
                new Currency { Code = "EUR", Name = "Euro", Symbol = "€", FlagIcon = "🇪🇺" },
                new Currency { Code = "GBP", Name = "British Pound", Symbol = "£", FlagIcon = "🇬🇧" },
                new Currency { Code = "JPY", Name = "Japanese Yen", Symbol = "¥", FlagIcon = "🇯🇵" },
                new Currency { Code = "LKR", Name = "Sri Lankan Rupee", Symbol = "Rs", FlagIcon = "🇱🇰" }
            };
        }
    }

    public class ExchangeRateResponse
    {
        public string Base { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Dictionary<string, double> Rates { get; set; } = new Dictionary<string, double>();
    }
}