using Docker.DotNet;

using DockerBackup.WebApi.Domain;
using DockerBackup.WebApi.Extensions;

using Hangfire;

namespace DockerBackup.WebApi.Jobs;

public sealed class BackupContainerJob
(
    IDockerClient docker,
    IRecurringJobManager recurringJob,
    IContainerBackupService containerBackupService
)
{
    public record Parameters(string ContainerName, List<string>? Directories = null);

    public static string GetJobName(string containerName) => containerName.Replace("/", string.Empty);

    public async Task DoBackup(Parameters parameters, CancellationToken cancellationToken = default)
    {
        var containerResponse = await docker.Containers.InspectContainerSafeAsync(parameters.ContainerName, cancellationToken);

        if (containerResponse is null)
        {
            recurringJob.RemoveIfExists(parameters.ContainerName);

            return;
        }

        await containerBackupService.CreateBackup(parameters.ContainerName, parameters.Directories, cancellationToken);
    }
}
