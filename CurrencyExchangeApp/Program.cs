using CurrencyExchangeApp.Client.Pages;
using CurrencyExchangeApp.Components;
using CurrencyExchangeApp.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Register HTTP client for server-side operations
builder.Services.AddHttpClient();

// Register client services for server-side prerendering
builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeApp.Client.Services.ExchangeRateApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CurrencyExchangeApp.Client._Imports).Assembly);

app.Run();