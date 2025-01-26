using Docker.DotNet;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DockerBackup.WebApi.HealthChecks;

public sealed class DockerHealthCheck(IDockerClient dockerClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        try
        {
            await dockerClient.Containers.ListContainersAsync(new()
            {
                Limit = 1,
            }, cancellationToken);
        }
        catch
        {
            isHealthy = false;
        }

        return isHealthy
            ? HealthCheckResult.Healthy("Can reach docker daemon")
            : HealthCheckResult.Unhealthy("Cannot reach docker daemon");
    }
}
