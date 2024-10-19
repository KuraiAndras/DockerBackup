using Docker.DotNet;
using Docker.DotNet.Models;

using DockerBackup.ApiClient;

namespace DockerBackup.WebApi.Controllers;

public sealed class ControllerImplementation(TimeProvider _time, IDockerClient _docker) : IController
{
    private static readonly string[] Summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    ];

    public async Task<SwaggerResponse<ICollection<Container>>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters()
        {
            All = true,
        }, cancellationToken);

        return containers
            .Select(c => new Container
            {
                Id = c.ID,
                Names = c.Names,
                Image = c.Image,
                Status = c.Status,
                BackupDirectories = c.Mounts
                    .Where(m => m.Type == "bind")
                    .Select(m => $"{m.Source}:{m.Destination}")
                    .ToList(),
            })
            .ToArray();
    }

    public async Task<SwaggerResponse<ICollection<WeatherForecast>>> GetWeatherForecastAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        {
            Date = _time.GetUtcNow().AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        })
        .ToList();

        return forecasts;
    }
}
