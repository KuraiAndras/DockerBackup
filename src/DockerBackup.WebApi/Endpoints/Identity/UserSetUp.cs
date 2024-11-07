using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class UserSetUp
{
    public static async Task<Results<Ok<UserSetUpResponse>, InternalServerError<ProblemDetails>>> Handle
    (
        UserManager<AppUser> userManager,
        CancellationToken cancellationToken
    )
    {
        var areThereAnyUsers = await userManager.Users.AnyAsync(cancellationToken);

        return TypedResults.Ok<UserSetUpResponse>(new(areThereAnyUsers));
    }
}
