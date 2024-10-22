using Docker.DotNet;
using Docker.DotNet.Models;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.Options;
using DockerBackup.WebApi.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Endpoints;

public sealed class CreateBackup
{
    public static async Task<Results
    <
        Ok<CreateBackupRespone>,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>
    >>
    Handle
    (
        [FromBody, BindRequired] CreateBackupRequest request,
        ApplicationDb db,
        IOptions<BackupOptions> backupOptions,
        IDockerClient docker,
        TimeProvider timeProvider,
        ILogger<CreateBackup> logger,
        CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<string>();
        ContainerListResponse? container = null;
        try
        {
            var now = timeProvider.GetLocalNow();

            var containers = await docker.Containers.ListContainersAsync(new()
            {
                All = true,
            }, cancellationToken);

            container = containers.SingleOrDefault(c => c.Names.Contains(request.ContainerName));

            if (container is null)
            {
                logger.LogWarning("Container {ContainerName} not found", request.ContainerName);
                return TypedResults.NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = $"Container {request.ContainerName} not found",
                });
            }

            var containerPathsToBackUp = container.Mounts
                .Where(m => m.Type == "bind")
                .Where(m => request.Directories.Contains(m.Source))
                .Select(m => m.Destination)
                .ToArray();

            var backupDirectory = Path.Combine(backupOptions.Value.BackupPath, request.ContainerName.TrimStart('/'), $"{now:yyyy-MM-ddTHH-mm-ss}");

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            if (container.State == "running")
            {
                await docker.Containers.StopContainerAsync(container.ID, new()
                {
                    WaitBeforeKillSeconds = request.WaitForContainerStopMs.HasValue
                        ? (uint)request.WaitForContainerStopMs
                        : backupOptions.Value.DefaultWaitForContainerStopMs,
                }, cancellationToken);
            }

            foreach (var containerPathToBackUp in containerPathsToBackUp)
            {
                await using DockerArchive archive = await docker.Containers.GetArchiveFromContainerAsync(container.ID, new()
                {
                    Path = containerPathToBackUp,
                }, statOnly: false, cancellationToken);

                var tarFileName = Path.Combine(backupDirectory, $"{containerPathToBackUp.TrimStart('/').Replace("/", "__")}.tar");
                backupFilePaths.Add(tarFileName);

                await using var folderTarBall = File.Create(tarFileName);
                await archive.Stream.CopyToAsync(folderTarBall, cancellationToken);
            }

            var containerBackup = new ContainerBackup
            {
                ContainerName = request.ContainerName,
                CreatedAt = now,
                Files = containerPathsToBackUp.Select((containerPathToBackUp, i) => new FileBackup
                {
                    FilePath = backupFilePaths[i],
                    ContainerPath = containerPathToBackUp,
                }).ToList(),
            };
            db.ContainerBackups.Add(containerBackup);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(new CreateBackupRespone
            (
                BackupId: containerBackup.Id
            ));
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

            logger.LogError(e, "Creating backup {ContainerName} failed", request.ContainerName);
            return new InternalServerError<ProblemDetails>(new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Creating backup failed",
            });
        }
        finally
        {
            if (container?.State == "running")
            {
                await docker.Containers.StartContainerAsync(container.ID, new(), cancellationToken);
            }
        }
    }
}
