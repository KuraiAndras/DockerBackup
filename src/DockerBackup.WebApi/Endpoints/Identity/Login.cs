using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class Login
{
    public static async Task<Results<EmptyHttpResult, ProblemHttpResult>> Handle
    (
        [FromBody] LoginRequest login,
        [FromQuery] bool? useCookies,
        [FromQuery] bool? useSessionCookies,
        SignInManager<AppUser> signInManager
    )
    {
        var isPersistent = useCookies == true && useSessionCookies != true;
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent, lockoutOnFailure: true);

        return result.Succeeded
            ? TypedResults.Empty // The signInManager already produced the needed response in the form of a cookie.
            : TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
    }
}
