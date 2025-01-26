using DockerBackup.WebApi.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DockerHealthCheckExtensions
{
    public static IHealthChecksBuilder AddDocker(
        this IHealthChecksBuilder builder,
        string name = "docker",
        HealthStatus failureStatus = HealthStatus.Unhealthy,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        tags ??= ["docker"];

        return builder.AddCheck<DockerHealthCheck>(name, failureStatus, tags, timeout);
    }
}
