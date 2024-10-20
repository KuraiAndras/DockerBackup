using Docker.DotNet;

using DockerBackup.WebApi.Controllers;
using DockerBackup.WebApi.Options;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());

builder.Services.AddScoped<IController, ControllerImplementation>();

builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection(BackupOptions.Section));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.UseOpenApi();
    app.UseSwaggerUi();

    var backupOptions = app.Services.GetRequiredService<IOptions<BackupOptions>>().Value;

    backupOptions.BackupPath = Path.Combine(Directory.GetCurrentDirectory(), "backups");
}

app.UseBlazorFrameworkFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.Run();
