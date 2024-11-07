using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using static DockerBackup.WebApi.Endpoints.Identity.IdentityMapper;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class Register
{
    public static async Task<Results<Ok, ValidationProblem, BadRequest<ProblemDetails>>> Handle
    (
        [FromBody] RegisterRequest registration,
        UserManager<AppUser> userManager,
        IIdentityService identityService,
        CancellationToken cancellationToken
    )
    {
        var canUsersRegister = await identityService.CanUsersRegister(cancellationToken);

        if (!canUsersRegister)
        {
            return TypedResults.BadRequest<ProblemDetails>(new()
            {
                Title = "Only one user can be registered."
            });
        }

        var result = await userManager.CreateAsync(
            new AppUser()
            {
                UserName = registration.UserName
            },
            registration.Password);

        return result.Succeeded
            ? TypedResults.Ok()
            : CreateValidationProblem(result);
    }
}
