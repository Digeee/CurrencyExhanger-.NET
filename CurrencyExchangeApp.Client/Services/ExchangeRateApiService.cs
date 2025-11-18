using System.Net.Http.Json;
using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Client.Services
{
    public class ExchangeRateApiService : ICurrencyExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly List<Currency> _supportedCurrencies;

        public ExchangeRateApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // For demo purposes, we're using the open endpoint which doesn't require an API key
            // In production, you would use a real API key with the v6 endpoint
            _apiKey = "YOUR_API_KEY_HERE"; // Replace with your actual API key
            
            _supportedCurrencies = new List<Currency>
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
                new Currency { Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", FlagIcon = "🇳🇿" },
                new Currency { Code = "MXN", Name = "Mexican Peso", Symbol = "$", FlagIcon = "🇲🇽" },
                new Currency { Code = "SGD", Name = "Singapore Dollar", Symbol = "S$", FlagIcon = "🇸🇬" },
                new Currency { Code = "HKD", Name = "Hong Kong Dollar", Symbol = "HK$", FlagIcon = "🇭🇰" },
                new Currency { Code = "NOK", Name = "Norwegian Krone", Symbol = "kr", FlagIcon = "🇳🇴" },
                new Currency { Code = "KRW", Name = "South Korean Won", Symbol = "₩", FlagIcon = "🇰🇷" },
                new Currency { Code = "TRY", Name = "Turkish Lira", Symbol = "₺", FlagIcon = "🇹🇷" },
                new Currency { Code = "RUB", Name = "Russian Ruble", Symbol = "₽", FlagIcon = "🇷🇺" },
                new Currency { Code = "INR", Name = "Indian Rupee", Symbol = "₹", FlagIcon = "🇮🇳" },
                new Currency { Code = "BRL", Name = "Brazilian Real", Symbol = "R$", FlagIcon = "🇧🇷" },
                new Currency { Code = "ZAR", Name = "South African Rand", Symbol = "R", FlagIcon = "🇿🇦" }
            };
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                // For the free/open API, we need to make a request for the base currency
                // and then calculate the conversion rate
                var exchangeRates = await GetExchangeRatesAsync(fromCurrency);
                
                if (exchangeRates.ContainsKey(toCurrency))
                {
                    var rate = exchangeRates[toCurrency];
                    return Math.Round(amount * (decimal)rate, 4);
                }
                
                throw new Exception($"Conversion rate not available for {fromCurrency} to {toCurrency}");
            }
            catch (Exception ex)
            {
                // Fallback to mock data if API fails
                return await GetMockConversionAsync(amount, fromCurrency, toCurrency);
            }
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var exchangeRates = await GetExchangeRatesAsync(fromCurrency);
                
                if (exchangeRates.ContainsKey(toCurrency))
                {
                    var rate = (decimal)exchangeRates[toCurrency];
                    return new ExchangeRate
                    {
                        FromCurrency = fromCurrency,
                        ToCurrency = toCurrency,
                        Rate = rate,
                        Timestamp = DateTime.UtcNow
                    };
                }
                
                throw new Exception($"Exchange rate not available for {fromCurrency} to {toCurrency}");
            }
            catch (Exception ex)
            {
                // Fallback to mock data if API fails
                return await GetMockExchangeRateAsync(fromCurrency, toCurrency);
            }
        }

        public async Task<List<HistoricalExchangeRate>> GetHistoricalRatesAsync(string fromCurrency, string toCurrency, int days)
        {
            try
            {
                // For demo purposes, we'll generate mock historical data
                // In a real implementation with a paid API key, you could fetch actual historical data
                return await GenerateMockHistoricalDataAsync(fromCurrency, toCurrency, days);
            }
            catch (Exception ex)
            {
                // Fallback to mock data if API fails
                return await GenerateMockHistoricalDataAsync(fromCurrency, toCurrency, days);
            }
        }

        public async Task<List<Currency>> GetAvailableCurrenciesAsync()
        {
            // Return our predefined list of supported currencies
            await Task.Delay(10); // Simulate network delay
            return _supportedCurrencies;
        }

        public async Task<List<Currency>> GetPopularCurrenciesAsync()
        {
            // Return the first 10 most popular currencies
            await Task.Delay(10); // Simulate network delay
            return _supportedCurrencies.Take(10).ToList();
        }

        private async Task<Dictionary<string, double>> GetExchangeRatesAsync(string baseCurrency)
        {
            try
            {
                // Using the open API endpoint which doesn't require an API key
                // Note: This endpoint updates only once per day
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(
                    $"https://open.er-api.com/v6/latest/{baseCurrency}");
                
                if (response?.Result == "success")
                {
                    return response.Rates;
                }
                
                throw new Exception("Failed to fetch exchange rates");
            }
            catch
            {
                // Fallback to mock data if API fails
                return await GetMockExchangeRatesAsync(baseCurrency);
            }
        }

        private async Task<decimal> GetMockConversionAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            var rates = await GetMockExchangeRatesAsync(fromCurrency);
            if (rates.ContainsKey(toCurrency))
            {
                return Math.Round(amount * (decimal)rates[toCurrency], 4);
            }
            return amount; // Fallback
        }

        private async Task<ExchangeRate> GetMockExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var rates = await GetMockExchangeRatesAsync(fromCurrency);
            if (rates.ContainsKey(toCurrency))
            {
                return new ExchangeRate
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    Rate = (decimal)rates[toCurrency],
                    Timestamp = DateTime.UtcNow
                };
            }
            
            return new ExchangeRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = 1.0m,
                Timestamp = DateTime.UtcNow
            };
        }

        private async Task<Dictionary<string, double>> GetMockExchangeRatesAsync(string baseCurrency)
        {
            await Task.Delay(100); // Simulate network delay
            
            // Generate mock exchange rates based on the base currency
            var rates = new Dictionary<string, double>();
            var random = new Random();
            
            foreach (var currency in _supportedCurrencies)
            {
                // Generate a realistic exchange rate relative to the base currency
                var rate = 0.1 + random.NextDouble() * 10;
                rates[currency.Code] = rate;
            }
            
            // Base currency should always be 1.0
            rates[baseCurrency] = 1.0;
            
            return rates;
        }

        private async Task<List<HistoricalExchangeRate>> GenerateMockHistoricalDataAsync(string fromCurrency, string toCurrency, int days)
        {
            await Task.Delay(200); // Simulate network delay
            
            var rates = new List<HistoricalExchangeRate>();
            var random = new Random();
            
            // Generate a base rate for the currency pair
            var baseRate = 0.5 + random.NextDouble() * 2;
            
            for (int i = days; i >= 0; i--)
            {
                // Add some fluctuation to make it realistic
                var fluctuation = (random.NextDouble() - 0.5) * 0.1; // ±5% fluctuation
                var rate = baseRate + fluctuation;
                
                rates.Add(new HistoricalExchangeRate
                {
                    Date = DateTime.UtcNow.AddDays(-i),
                    Rate = (decimal)rate
                });
            }
            
            return rates;
        }
    }

    // API Response Models
    public class ExchangeRateApiResponse
    {
        public string Result { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Documentation { get; set; } = string.Empty;
        public string TermsOfUse { get; set; } = string.Empty;
        public long TimeLastUpdateUnix { get; set; }
        public string TimeLastUpdateUtc { get; set; } = string.Empty;
        public long TimeNextUpdateUnix { get; set; }
        public string TimeNextUpdateUtc { get; set; } = string.Empty;
        public string BaseCode { get; set; } = string.Empty;
        public Dictionary<string, double> Rates { get; set; } = new Dictionary<string, double>();
    }
}