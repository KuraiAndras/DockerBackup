using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DockerBackup.WebClient;
using MudBlazor.Services;
using DockerBackup.ApiClient;
using System.Text.Json;
using DockerBackup.WebClient.Interop;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IClient, Client>(sp =>
{
    var client = new Client(sp.GetRequiredService<HttpClient>());

    client.JsonSerializerSettings.WriteIndented = false;
    client.JsonSerializerSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    client.JsonSerializerSettings.PropertyNameCaseInsensitive = true;

    return client;
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;

    config.SnackbarConfiguration.MaxDisplayedSnackbars = 5;
});

builder.Services.AddScoped<IClipboardService, ClipboardService>();

await builder.Build().RunAsync();
