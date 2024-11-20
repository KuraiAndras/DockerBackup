using Docker.DotNet;

using DockerBackup.WebApi.Domain;
using DockerBackup.WebApi.Options;

using Hangfire;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Jobs;

public sealed class UpdateContainerConfigurations(IOptions<BackupOptions> options, IDockerClient docker, IRecurringJobManager recurringJob)
{
    public const string JobName = "update-container-backup-configurations";

    public async Task Handle(CancellationToken cancellationToken = default)
    {
        var containers = await docker.Containers.ListContainersAsync(new() { All = true }, cancellationToken);

        foreach (var container in containers.Select(c => new ContainerBackupInfo(c, options.Value)).Where(c => c.NeedsBackup))
        {
            var containerName = container.Name;
            var parameters = new BackupContainerJob.Parameters(containerName);
            recurringJob.AddOrUpdate<BackupContainerJob>
            (
                containerName.Replace("/", string.Empty),
                x => x.DoBackup(parameters, CancellationToken.None),
                container.Cron
            );
        }
    }
}
