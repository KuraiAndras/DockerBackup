using Docker.DotNet;

using DockerBackup.WebApi.Extensions;
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

        foreach (var container in containers.Where(c => c.NeedsBackup()))
        {
            var containerName = container.Names.First();
            var parameters = new BackupJobParameters(containerName);
            recurringJob.AddOrUpdate<BackupContainerJob>
            (
                container.Names.First().Replace("/", string.Empty),
                x => x.DoBackup(parameters, CancellationToken.None),
                container.GetBackupCron() ?? options.Value.Cron
            );
        }
    }
}
