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
        IOptions<ServerOptions> serverOptions,
        IOptions<BackupOptions> backupOptions,
        IDockerClient docker,
        TimeProvider timeProvider,
        ILogger<CreateBackup> logger,
        CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<string>();
        ContainerInspectResponse? container = null;
        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;

            container = await docker.Containers.InspectContainerAsync(request.ContainerName, cancellationToken);

            var backupDirectory = Path.Combine(serverOptions.Value.BackupPath, request.ContainerName.TrimStart('/'), $"{now:yyyy-MM-ddTHH-mm-ss}");

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            if (container.State.Status == "running")
            {
                await docker.Containers.StopContainerAsync(container.ID, new()
                {
                    WaitBeforeKillSeconds = request.WaitForContainerStopMs.HasValue
                        ? (uint)request.WaitForContainerStopMs
                        : backupOptions.Value.DefaultWaitForContainerStopMs,
                }, cancellationToken);
            }

            var backedUpContainerPaths = new List<string>();

            foreach (var containerPathToBackUp in request.Directories ?? container.Config.Labels
                .Where(l => l.Key.StartsWith("backup.dir"))
                .Select(l => l.Value))
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
                ContainerName = request.ContainerName,
                CreatedAt = now,
                Files = backedUpContainerPaths.Select((containerPathToBackUp, i) => new FileBackup
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
        catch (DockerContainerNotFoundException e)
        {
            logger.LogWarning(e, "Container {ContainerName} not found", request.ContainerName);
            return TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = $"Container {request.ContainerName} not found",
            });
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
            if (container?.State.Status == "running")
            {
                await docker.Containers.StartContainerAsync(container.ID, new(), cancellationToken);
            }
        }
    }
}
