using System.Reflection;

using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.WebApi.Endpoints;

public sealed class GetVersion
{
    public static Results<Ok<GetVersionResponse>, InternalServerError<ProblemDetails>> Handle() =>
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString() is string version
            ? TypedResults.Ok(new GetVersionResponse(version))
            : TypedResults.InternalServerError(new ProblemDetails()
            {
                Title = "Could not get the current version",
            });
}
