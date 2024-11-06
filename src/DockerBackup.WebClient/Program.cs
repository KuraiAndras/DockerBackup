using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DockerBackup.WebClient;
using MudBlazor.Services;
using DockerBackup.ApiClient;
using DockerBackup.WebClient.Interop;
using MudBlazor;
using DockerBackup.WebClient.Auth;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CookieHandler>();

builder.Services
    .AddHttpClient<IClient, Client>(http => http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped<CookieAuthenticationStateProvider, CookieAuthenticationStateProvider>(sp => (CookieAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

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
