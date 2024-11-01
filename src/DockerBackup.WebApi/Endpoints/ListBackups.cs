using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints;

public sealed class ListBackups
{
    public static async Task<Ok<ListContainerBackupResponse[]>> Handle(ApplicationDb db, CancellationToken cancellationToken = default) =>
        TypedResults.Ok(
            await db.ContainerBackups
                .GroupBy(b => b.ContainerName)
                .Select(b => new ListContainerBackupResponse(
                    b.First().Id,
                    b.Key,
                    b.OrderBy(b => b.CreatedAt).Last().CreatedAt,
                    b.Count(),
                    b.Sum(x => x.Files.Sum(y => y.SizeInBytes))
                ))
                .ToArrayAsync(cancellationToken));
}
