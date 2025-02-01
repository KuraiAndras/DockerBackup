using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints;

public sealed class DeleteBackup
{
    public static async Task<Results
    <
        Ok,
        InternalServerError<ProblemDetails>
    >>
    Handle
    (
        [FromRoute, BindRequired] Guid backupId,
        ApplicationDb db,
        ILogger<DeleteBackup> logger,
        CancellationToken cancellationToken)
    {
        // TODO: make this a job?

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var backupFilePaths = await db.FileBackups
            .Where(f => f.ContainerBackupId == backupId)
            .Select(f => f.FilePath)
            .ToListAsync(cancellationToken);

        await db.FileBackups
            .Where(f => f.ContainerBackupId == backupId)
            .ExecuteDeleteAsync(cancellationToken);

        await db.ContainerBackups
            .Where(c => c.Id == backupId)
            .ExecuteDeleteAsync(cancellationToken);

        var directoriesToDelete = backupFilePaths.Select(Path.GetDirectoryName).Distinct();

        foreach (var directory in directoriesToDelete)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                logger.LogInformation("Deleted directory: {Directory}", directory);
            }
            else
            {
                logger.LogWarning("Tried to delete directory: {Directory} but it does not exist", directory);
            }
        }

        await transaction.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
