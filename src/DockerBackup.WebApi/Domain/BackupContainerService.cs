using System.IO.Compression;

using Docker.DotNet;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.Options;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Domain;

public sealed class ContainerBackupService
(
    IOptions<ServerOptions> serverOptions,
    IOptions<BackupOptions> backupOptions,
    ILogger<ContainerBackupService> logger,
    IDockerClient docker,
    TimeProvider timeProvider,
    ApplicationDb db
 ) : IContainerBackupService
{
    public async Task CreateBackup(string containerName, List<string>? directories, CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<FileInfo>();
        ContainerBackupInfo? container = null;

        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;

            var containerResponse = await docker.Containers.InspectContainerSafeAsync(containerName, cancellationToken)
                ?? throw new DockerContainerNotFoundException(System.Net.HttpStatusCode.NotFound, "not found"); // TODO: use custom exception

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

            foreach (var containerPathToBackUp in directories is { Count: > 0 }
                ? directories
                : container.Directories)
            {
                // TODO: check if path exists in directory?
                await using DockerArchive archive = await docker.Containers.GetArchiveFromContainerAsync(container.Id, new()
                {
                    Path = containerPathToBackUp,
                }, statOnly: false, cancellationToken);

                var tarFileName = ContainerBackupInfo.GetBackupFileName(backupDirectory, containerPathToBackUp, container.Compress);
                backupFilePaths.Add(new(tarFileName));

                await using var tarStream = container.Compress
                    ? new GZipStream(File.Create(tarFileName), container.CompressionMode)
                    : File.Create(tarFileName) as Stream;

                await archive.Stream.CopyToAsync(tarStream, cancellationToken);

                backedUpContainerPaths.Add(containerPathToBackUp);
            }

            var containerBackup = new ContainerBackup
            {
                ContainerName = containerName,
                CreatedAt = now,
                Files = [.. backedUpContainerPaths.Select((containerPathToBackUp, i) => FileBackup.Create
                (
                    filePath: backupFilePaths[i].FullName,
                    containerPath: containerPathToBackUp,
                    sizeInBytes: backupFilePaths[i].Length
                ))],
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

            logger.LogError(e, "Creating backup {ContainerName} failed", containerName);
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
