using Docker.DotNet;

using DockerBackup.WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddControllers();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IDockerClient>(_ => new DockerClientConfiguration().CreateClient());

builder.Services.AddScoped<IController, ControllerImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseBlazorFrameworkFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.Run();
