using DockerBackup.ApiClient;
using DockerBackup.WebApi.Jobs;
using DockerBackup.WebApi.Results;

using Hangfire;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DockerBackup.WebApi.Endpoints;

public sealed class CreateBackup
{
    public static Results
    <
        Ok,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>
    >
    Handle
    (
        [FromBody, BindRequired] CreateBackupRequest request,
        IBackgroundJobClient backgroundJob)
    {
        var parameters = new BackupJobParameters(request.ContainerName, request.Directories?.ToList());
        backgroundJob.Enqueue<BackupContainerJob>(x => x.DoBackup(parameters, CancellationToken.None));
        return TypedResults.Ok();
    }
}
