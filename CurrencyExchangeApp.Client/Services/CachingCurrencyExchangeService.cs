using CurrencyExchangeApp.Client.Models;

namespace CurrencyExchangeApp.Client.Services
{
    public class CachingCurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrencyExchangeService _innerService;
        private readonly Dictionary<string, CacheEntry<decimal>> _conversionCache;
        private readonly Dictionary<string, CacheEntry<ExchangeRate>> _rateCache;
        private readonly Dictionary<string, CacheEntry<List<HistoricalExchangeRate>>> _historicalCache;
        private readonly Dictionary<string, CacheEntry<List<Currency>>> _currencyCache;
        private readonly TimeSpan _cacheDuration;

        public CachingCurrencyExchangeService(ICurrencyExchangeService innerService)
        {
            _innerService = innerService;
            _conversionCache = new Dictionary<string, CacheEntry<decimal>>();
            _rateCache = new Dictionary<string, CacheEntry<ExchangeRate>>();
            _historicalCache = new Dictionary<string, CacheEntry<List<HistoricalExchangeRate>>>();
            _currencyCache = new Dictionary<string, CacheEntry<List<Currency>>>();
            
            // Cache for 5 minutes by default
            _cacheDuration = TimeSpan.FromMinutes(5);
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            var cacheKey = $"{amount}_{fromCurrency}_{toCurrency}";
            
            // Check if we have a valid cached result
            if (TryGetCachedValue(_conversionCache, cacheKey, out var cachedConversion))
            {
                return cachedConversion;
            }
            
            // Get fresh data from the inner service
            var result = await _innerService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency);
            
            // Cache the result
            _conversionCache[cacheKey] = new CacheEntry<decimal>(result, DateTime.UtcNow.Add(_cacheDuration));
            
            return result;
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var cacheKey = $"{fromCurrency}_{toCurrency}";
            
            // Check if we have a valid cached result
            if (TryGetCachedValue(_rateCache, cacheKey, out var cachedRate))
            {
                return cachedRate;
            }
            
            // Get fresh data from the inner service
            var result = await _innerService.GetExchangeRateAsync(fromCurrency, toCurrency);
            
            // Cache the result
            _rateCache[cacheKey] = new CacheEntry<ExchangeRate>(result, DateTime.UtcNow.Add(_cacheDuration));
            
            return result;
        }

        public async Task<List<HistoricalExchangeRate>> GetHistoricalRatesAsync(string fromCurrency, string toCurrency, int days)
        {
            var cacheKey = $"{fromCurrency}_{toCurrency}_{days}";
            
            // Check if we have a valid cached result
            if (TryGetCachedValue(_historicalCache, cacheKey, out var cachedHistorical))
            {
                return cachedHistorical;
            }
            
            // Get fresh data from the inner service
            var result = await _innerService.GetHistoricalRatesAsync(fromCurrency, toCurrency, days);
            
            // Cache the result
            _historicalCache[cacheKey] = new CacheEntry<List<HistoricalExchangeRate>>(result, DateTime.UtcNow.Add(_cacheDuration));
            
            return result;
        }

        public async Task<List<Currency>> GetAvailableCurrenciesAsync()
        {
            const string cacheKey = "available_currencies";
            
            // Check if we have a valid cached result
            if (TryGetCachedValue(_currencyCache, cacheKey, out var cachedCurrencies))
            {
                return cachedCurrencies;
            }
            
            // Get fresh data from the inner service
            var result = await _innerService.GetAvailableCurrenciesAsync();
            
            // Cache the result
            _currencyCache[cacheKey] = new CacheEntry<List<Currency>>(result, DateTime.UtcNow.Add(_cacheDuration));
            
            return result;
        }

        public async Task<List<Currency>> GetPopularCurrenciesAsync()
        {
            const string cacheKey = "popular_currencies";
            
            // Check if we have a valid cached result
            if (TryGetCachedValue(_currencyCache, cacheKey, out var cachedCurrencies))
            {
                return cachedCurrencies;
            }
            
            // Get fresh data from the inner service
            var result = await _innerService.GetPopularCurrenciesAsync();
            
            // Cache the result
            _currencyCache[cacheKey] = new CacheEntry<List<Currency>>(result, DateTime.UtcNow.Add(_cacheDuration));
            
            return result;
        }

        private bool TryGetCachedValue<T>(Dictionary<string, CacheEntry<T>> cache, string key, out T value)
        {
            value = default(T)!;
            
            if (cache.TryGetValue(key, out var entry))
            {
                // Check if the cache entry is still valid
                if (entry.ExpirationTime > DateTime.UtcNow)
                {
                    value = entry.Value;
                    return true;
                }
                else
                {
                    // Remove expired entry
                    cache.Remove(key);
                }
            }
            
            return false;
        }

        // Clear all cached data
        public void ClearCache()
        {
            _conversionCache.Clear();
            _rateCache.Clear();
            _historicalCache.Clear();
            _currencyCache.Clear();
        }

        // Clear specific cache entry
        public void ClearCacheEntry(string cacheKey)
        {
            _conversionCache.Remove(cacheKey);
            _rateCache.Remove(cacheKey);
            _historicalCache.Remove(cacheKey);
            _currencyCache.Remove(cacheKey);
        }
    }

    // Cache entry wrapper
    public class CacheEntry<T>
    {
        public T Value { get; set; }
        public DateTime ExpirationTime { get; set; }

        public CacheEntry(T value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }
}