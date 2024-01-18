using Docker.DotNet;

namespace DockerBackup;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = new DockerClientConfiguration()
            .CreateClient();

        var images = await client.Images.ListImagesAsync(new() { All = true }, stoppingToken);

        _logger.LogInformation("Images: {images}", images.SelectMany(i => i.RepoTags));

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
