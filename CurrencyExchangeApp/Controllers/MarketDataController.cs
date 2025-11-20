using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDataController : ControllerBase
    {
        // GET: api/marketdata/currencies
        [HttpGet("currencies")]
        public IActionResult GetAvailableCurrencies()
        {
            var currencies = new List<Currency>
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

            return Ok(currencies);
        }

        // GET: api/marketdata/rates/{fromCurrency}/{toCurrency}
        [HttpGet("rates/{fromCurrency}/{toCurrency}")]
        public IActionResult GetExchangeRate(string fromCurrency, string toCurrency)
        {
            // In a real application, this would fetch from an external API
            // For demo purposes, we'll return a simulated rate
            var random = new Random();
            var rate = 0.5 + random.NextDouble() * 2; // Random rate between 0.5 and 2.5
            
            var exchangeRate = new ExchangeRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = (decimal)rate,
                Timestamp = DateTime.UtcNow
            };

            return Ok(exchangeRate);
        }

        // GET: api/marketdata/convert/{fromCurrency}/{toCurrency}/{amount}
        [HttpGet("convert/{fromCurrency}/{toCurrency}/{amount}")]
        public IActionResult ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            // In a real application, this would use actual exchange rates
            // For demo purposes, we'll use a simulated conversion
            var random = new Random();
            var rate = 0.5 + random.NextDouble() * 2; // Random rate between 0.5 and 2.5
            var convertedAmount = amount * (decimal)rate;

            return Ok(convertedAmount);
        }

        // GET: api/marketdata/historical/{fromCurrency}/{toCurrency}
        [HttpGet("historical/{fromCurrency}/{toCurrency}")]
        public IActionResult GetHistoricalRates(string fromCurrency, string toCurrency)
        {
            // Simulate historical data
            var historicalRates = new List<HistoricalExchangeRate>();
            var random = new Random();
            var baseRate = 0.5 + random.NextDouble() * 2; // Random base rate between 0.5 and 2.5

            for (int i = 30; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                var rate = baseRate + (random.NextDouble() - 0.5) * 0.1; // Small variation
                
                historicalRates.Add(new HistoricalExchangeRate
                {
                    Date = date,
                    Rate = (decimal)rate
                });
            }

            return Ok(historicalRates);
        }
    }
}