using Docker.DotNet;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Endpoints;
using DockerBackup.WebApi.Endpoints.Identity;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.HangfireDashboard;
using DockerBackup.WebApi.Jobs;
using DockerBackup.WebApi.Options;

using Hangfire;
using Hangfire.Storage.SQLite;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(ServerOptions.Section));
builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection(BackupOptions.Section));

builder.Services.AddDbContext<ApplicationDb>((sp, options) =>
{
    var backupOptions = sp.GetRequiredService<IOptions<ServerOptions>>().Value;
    options.UseSqlite($"Data Source={backupOptions.DatabaseFilePath()}");
});

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<ApplicationDb>()
    .AddApiEndpoints();

builder.Services.AddScoped<DbSetup>();
builder.Services.AddScoped<SetupContainerScan>();

builder.Services
    .AddHangfire((sp, configuration) =>
    {
        var serverOptions = sp.GetRequiredService<IOptions<ServerOptions>>().Value;

        configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage(serverOptions.HangfireDatabaseFilePath());
    })
    .AddHangfireServer();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDb>()
    .AddHangfire(o => o.MinimumAvailableServers = 1)
    .AddDocker();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.UseOpenApi();
    app.UseSwaggerUi();

    var backupOptions = app.Services.GetRequiredService<IOptions<ServerOptions>>().Value;

    backupOptions.BackupPath = Path.Combine(Directory.GetCurrentDirectory(), "backups");
    backupOptions.ConfigDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "config");

    EnsureDirectoryExists(backupOptions.BackupPath);
    EnsureDirectoryExists(backupOptions.ConfigDirectoryPath);

    static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}

app.UseBlazorFrameworkFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapHealthChecks("/api/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard(options: new()
{
    IgnoreAntiforgeryToken = true,
    Authorization = [new DefaultAuthorizationFilter()],
});

app.MapDockerBackup();

await app.RunJob<DbSetup>(s => s.Setup());
await app.RunJob<SetupContainerScan>(s => s.Setup());

await app.RunAsync();
