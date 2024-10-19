using DockerBackup.ApiClient;

namespace DockerBackup.WebApi.Controllers;

public sealed class ControllerImplementation(TimeProvider _time) : IController
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
