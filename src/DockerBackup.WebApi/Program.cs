using Docker.DotNet;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Endpoints;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.Options;

using Hangfire;
using Hangfire.Storage.SQLite;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(ServerOptions.Section));
builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection(BackupOptions.Section));

builder.Services.AddDbContext<ApplicationDb>((sp, options) =>
{
    var backupOptions = sp.GetRequiredService<IOptions<ServerOptions>>().Value;
    options.UseSqlite($"Data Source={backupOptions.DatabaseFilePath()}");
});

builder.Services.AddScoped<IDbSetup, DbSetup>();

// Add Hangfire services.
builder.Services.AddHangfire((sp, configuration) =>
{
    var backupOptions = sp.GetRequiredService<IOptions<ServerOptions>>().Value;
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSQLiteStorage(backupOptions.HangfireDatabaseFilePath());
});

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

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

app.UseHangfireDashboard();

app.UseBlazorFrameworkFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.MapDockerBackup();

await app.RunJobAsync<IDbSetup>(async s => await s.Setup());

await app.RunAsync();
