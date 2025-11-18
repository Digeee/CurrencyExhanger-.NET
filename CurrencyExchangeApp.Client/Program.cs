using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CurrencyExchangeApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register services
builder.Services.AddScoped<ExchangeRateApiService>();
builder.Services.AddScoped<ICurrencyExchangeService>(sp => 
    new CachingCurrencyExchangeService(
        sp.GetRequiredService<ExchangeRateApiService>()));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();