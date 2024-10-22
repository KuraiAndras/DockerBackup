using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints;

public sealed class GetBackupsForContainer
{
    public static async Task<Results
    <
        Ok<ContainerBackupResponse[]>,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>
    >>
    Handle([FromRoute, BindRequired] Guid containerId, ApplicationDb db, CancellationToken cancellationToken = default)
    {
        var containers = await db.ContainerBackups
            .Where(b => b.Id == containerId)
            .Select(b => new ContainerBackupResponse
            (
                b.Id,
                b.ContainerName,
                b.CreatedAt,
                b.Files.Select(f => new FileBackupResponse(f.Id, f.FilePath, f.ContainerPath)).ToArray()
            ))
            .ToArrayAsync(cancellationToken);

        return containers is null
            ? TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = $"Container {containerId} not found",
            })
            : TypedResults.Ok(containers);
    }
}
