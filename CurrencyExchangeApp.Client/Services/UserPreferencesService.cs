using CurrencyExchangeApp.Client.Models;
using System.Text.Json;

namespace CurrencyExchangeApp.Client.Services
{
    public interface IUserPreferencesService
    {
        UserPreferences GetPreferences();
        void SavePreferences(UserPreferences preferences);
        event Action<UserPreferences>? PreferencesChanged;
    }

    public class UserPreferences
    {
        public string DefaultFromCurrency { get; set; } = "USD";
        public string DefaultToCurrency { get; set; } = "EUR";
        public string Theme { get; set; } = "light";
        public bool NotificationsEnabled { get; set; } = true;
        public List<string> FavoriteCurrencies { get; set; } = new List<string>();
        public string ChartPeriod { get; set; } = "7D";
    }

    public class UserPreferencesService : IUserPreferencesService
    {
        private const string PreferencesKey = "user_preferences";
        private UserPreferences? _preferences;
        private readonly ILocalStorageService _localStorage;

        public event Action<UserPreferences>? PreferencesChanged;

        public UserPreferencesService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public UserPreferences GetPreferences()
        {
            if (_preferences != null)
                return _preferences;

            var preferencesJson = _localStorage.GetItemAsStringAsync(PreferencesKey).Result;
            if (!string.IsNullOrEmpty(preferencesJson))
            {
                try
                {
                    _preferences = JsonSerializer.Deserialize<UserPreferences>(preferencesJson);
                }
                catch
                {
                    // If deserialization fails, use default preferences
                    _preferences = new UserPreferences();
                }
            }
            else
            {
                _preferences = new UserPreferences();
            }

            return _preferences;
        }

        public void SavePreferences(UserPreferences preferences)
        {
            _preferences = preferences;
            var preferencesJson = JsonSerializer.Serialize(preferences);
            _localStorage.SetItemAsStringAsync(PreferencesKey, preferencesJson).Wait();
            PreferencesChanged?.Invoke(preferences);
        }
    }
}