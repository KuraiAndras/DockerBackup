using System.Web;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

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
    Handle([FromRoute, BindRequired] string containerName, ApplicationDb db, CancellationToken cancellationToken = default)
    {
        containerName = HttpUtility.UrlDecode(containerName);

        var containers = await db.ContainerBackups
            .Where(b => b.ContainerName == containerName)
            .Select(b => new ContainerBackupResponse
            (
                b.Id,
                b.CreatedAt,
                b.Files
                    .OrderBy(f => f.ContainerPath)
                    .Select(f => new FileBackupResponse
                        (
                            f.Id,
                            f.FilePath,
                            f.ContainerPath,
                            f.SizeInBytes
                        ))
                    .ToArray()
            ))
            .ToArrayAsync(cancellationToken);

        return containers is null
            ? TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = $"Container {containerName} not found",
            })
            : TypedResults.Ok(containers.OrderByDescending(c => c.CreatedAt).ToArray());
    }
}
