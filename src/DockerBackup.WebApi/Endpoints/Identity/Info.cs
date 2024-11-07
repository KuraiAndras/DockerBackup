using System.Security.Claims;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class Info
{
    public static async Task<Results
    <
        Ok<InfoResponse>,
        ValidationProblem,
        NotFound,
        InternalServerError<ProblemDetails>
    >>
    Handle(ClaimsPrincipal claimsPrincipal, UserManager<AppUser> userManager)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var userName = await userManager.GetUserNameAsync(user);

        return userName is not null
            ? TypedResults.Ok<InfoResponse>(new()
            {
                UserName = userName,
            })
            : new InternalServerError<ProblemDetails>(new()
            {
                Title = "Users must have a name.",
            });
    }
}
