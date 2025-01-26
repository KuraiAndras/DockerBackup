using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints;

public sealed class GetOverallBackupStorageSize
{
    public static async Task<Ok<GetOverallBackupStorageSizeResponse>> Handle([FromServices] ApplicationDb db, CancellationToken cancellationToken)
    {
        var sumSizeInBytes = await db.FileBackups.SumAsync(f => f.SizeInBytes, cancellationToken);

        return TypedResults.Ok(new GetOverallBackupStorageSizeResponse(sumSizeInBytes));
    }
}
