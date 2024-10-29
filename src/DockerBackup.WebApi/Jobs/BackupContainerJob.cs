using Docker.DotNet;
using Docker.DotNet.Models;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.Options;

using Hangfire;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Jobs;

public sealed class BackupContainerJob
(
    IOptions<ServerOptions> serverOptions,
    IOptions<BackupOptions> backupOptions,
    ILogger<BackgroundJob> logger,
    IDockerClient docker,
    IRecurringJobManager recurringJob,
    TimeProvider timeProvider,
    ApplicationDb db
)
{
    public async Task DoBackup(BackupJobParameters parameters, CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<string>();
        ContainerInspectResponse? container = null;
        
        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;

            container = await docker.Containers.InspectContainerSafeAsync(parameters.ContainerName, cancellationToken);

            if (container is null)
            {
                recurringJob.RemoveIfExists(parameters.ContainerName);

                return;
            }

            var backupDirectory = Path.Combine(serverOptions.Value.BackupPath, parameters.ContainerName.TrimStart('/'), $"{now:yyyy-MM-ddTHH-mm-ss}");

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            if (container.State.Status == "running")
            {
                await docker.Containers.StopContainerAsync(container.ID, new()
                {
                    // TODO: wait time from config
                    WaitBeforeKillSeconds = backupOptions.Value.DefaultWaitForContainerStopMs,
                }, cancellationToken);
            }

            var backedUpContainerPaths = new List<string>();

            foreach (var containerPathToBackUp in parameters.Directories is { Count: > 0 }
                ? parameters.Directories
                : container.Config.GetBackupPaths())
            {
                await using DockerArchive archive = await docker.Containers.GetArchiveFromContainerAsync(container.ID, new()
                {
                    Path = containerPathToBackUp,
                }, statOnly: false, cancellationToken);

                var tarFileName = Path.Combine(backupDirectory, $"{containerPathToBackUp.TrimStart('/').Replace("/", "__")}.tar");
                backupFilePaths.Add(tarFileName);

                await using var folderTarBall = File.Create(tarFileName);
                await archive.Stream.CopyToAsync(folderTarBall, cancellationToken);

                backedUpContainerPaths.Add(containerPathToBackUp);
            }

            var containerBackup = new ContainerBackup
            {
                ContainerName = parameters.ContainerName,
                CreatedAt = now,
                Files = backedUpContainerPaths.Select((containerPathToBackUp, i) => new FileBackup
                {
                    FilePath = backupFilePaths[i],
                    ContainerPath = containerPathToBackUp,
                }).ToList(),
            };
            db.ContainerBackups.Add(containerBackup);

            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            foreach (var backupFilePath in backupFilePaths)
            {
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }
            }

            logger.LogError(e, "Creating backup {ContainerName} failed", parameters.ContainerName);
            throw;
        }
        finally
        {
            if (container?.State.Status == "running")
            {
                await docker.Containers.StartContainerAsync(container.ID, new(), cancellationToken);
            }
        }
    }
}
