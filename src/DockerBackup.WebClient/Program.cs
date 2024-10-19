using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DockerBackup.WebClient;
using MudBlazor.Services;
using DockerBackup.ApiClient;
using System.Text.Json;
using DockerBackup.WebClient.Interop;

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

builder.Services.AddMudServices();

builder.Services.AddScoped<IClipboardService, ClipboardService>();

await builder.Build().RunAsync();
