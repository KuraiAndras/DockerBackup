using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class UserSetUp
{
    public static async Task<Results<Ok<UserSetUpResponse>, InternalServerError<ProblemDetails>>> Handle
    (
        IIdentityService identityService,
        CancellationToken cancellationToken
    )
    {
        var areThereAnyUsers = await identityService.CanUsersRegister(cancellationToken);

        return TypedResults.Ok<UserSetUpResponse>(new(!areThereAnyUsers));
    }
}
