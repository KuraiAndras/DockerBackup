using System.IO.Compression;

using Docker.DotNet;

using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Domain;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints;

public sealed class RestoreContainer
{
    public static async Task<Results
    <
        Ok,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>
    >>
    Handle
    (
        [FromRoute] Guid backupId,
        IDockerClient docker,
        ApplicationDb db,
        ILogger<RestoreContainer> logger,
        CancellationToken cancellationToken = default
    )
    {
        // TODO: make this a job
        var backup = await db.ContainerBackups
            .Where(b => b.Id == backupId)
            .Select(b => new
            {
                b.ContainerName,
                Files = b.Files.Select(f => new { f.FilePath, f.ContainerPath }),
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (backup is null)
        {
            logger.LogError("Backup {BackupId} not found", backupId);
            return TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = $"Backup {backupId} not found",
            });
        }

        var container = await docker.Containers.InspectContainerAsync(backup.ContainerName, cancellationToken);

        try
        {
            if (container.State.Status == "running")
            {
                await docker.Containers.StopContainerAsync(container.ID, new(), cancellationToken);
            }

            foreach (var file in backup.Files)
            {
                await using var fileStream = ContainerBackupInfo.IsFileCompressed(file.FilePath) 
                    ? new GZipStream(File.OpenRead(file.FilePath), CompressionMode.Decompress)
                    : File.OpenRead(file.FilePath) as Stream;

                await docker.Containers.ExtractArchiveToContainerAsync(container.ID, new()
                {
                    AllowOverwriteDirWithFile = true,
                    Path = "/",
                }, fileStream, cancellationToken);
            }

            return TypedResults.Ok();
        }
        catch (DockerContainerNotFoundException e)
        {
            logger.LogError(e, "Container {ContainerName} not found", backup.ContainerName);
            return TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = $"Container {backup.ContainerName} not found",
            });
        }
        finally
        {
            if (container.State.Status == "running")
            {
                await docker.Containers.StartContainerAsync(container.ID, new(), cancellationToken);
            }
        }
    }
}
