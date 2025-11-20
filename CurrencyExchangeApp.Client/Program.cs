using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CurrencyExchangeApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ICurrencyExchangeService, ExchangeRateApiService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();

await builder.Build().RunAsync();