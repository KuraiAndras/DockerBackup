using Docker.DotNet;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Domain;
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
    public record Parameters(string ContainerName, List<string>? Directories = null);

    public static string GetJobName(string containerName) => containerName.Replace("/", string.Empty);

    public async Task DoBackup(Parameters parameters, CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<FileInfo>();
        ContainerBackupInfo? container = null;

        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;

            var containerResponse = await docker.Containers.InspectContainerSafeAsync(parameters.ContainerName, cancellationToken);

            if (containerResponse is null)
            {
                recurringJob.RemoveIfExists(parameters.ContainerName);

                return;
            }

            container = new ContainerBackupInfo(containerResponse, backupOptions.Value);

            var backupDirectory = container.GetBackupDirectory(serverOptions.Value, now);

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            if (container.IsRunning)
            {
                await docker.Containers.StopContainerAsync(container.Id, new()
                {
                    WaitBeforeKillSeconds = container.WaitBeforeKill,
                }, cancellationToken);
            }

            var backedUpContainerPaths = new List<string>();

            foreach (var containerPathToBackUp in parameters.Directories is { Count: > 0 }
                ? parameters.Directories
                : container.Directories)
            {
                await using DockerArchive archive = await docker.Containers.GetArchiveFromContainerAsync(container.Id, new()
                {
                    Path = containerPathToBackUp,
                }, statOnly: false, cancellationToken);

                var tarFileName = ContainerBackupInfo.GetBackupFileName(backupDirectory, containerPathToBackUp);
                backupFilePaths.Add(new(tarFileName));

                await using var folderTarBall = File.Create(tarFileName);
                await archive.Stream.CopyToAsync(folderTarBall, cancellationToken);

                backedUpContainerPaths.Add(containerPathToBackUp);
            }

            var containerBackup = new ContainerBackup
            {
                ContainerName = parameters.ContainerName,
                CreatedAt = now,
                Files = backedUpContainerPaths.Select((containerPathToBackUp, i) => FileBackup.Create
                (
                    filePath: backupFilePaths[i].FullName,
                    containerPath: containerPathToBackUp,
                    sizeInBytes: backupFilePaths[i].Length
                )).ToList(),
            };
            db.ContainerBackups.Add(containerBackup);

            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            foreach (var backupFilePath in backupFilePaths)
            {

                if (backupFilePath.Exists)
                {
                    backupFilePath.Delete();
                }
            }

            logger.LogError(e, "Creating backup {ContainerName} failed", parameters.ContainerName);
            throw;
        }
        finally
        {
            if (container?.IsRunning == true)
            {
                await docker.Containers.StartContainerAsync(container.Id, new(), cancellationToken);
            }
        }
    }
}
