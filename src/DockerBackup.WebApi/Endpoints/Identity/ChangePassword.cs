using System.Security.Claims;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using static DockerBackup.WebApi.Endpoints.Identity.IdentityMapper;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class ChangePassword
{
    public static async Task<Results<Ok, ValidationProblem, NotFound>> Handle
    (
        [FromBody] ChangePasswordRequest infoRequest,
        ClaimsPrincipal claimsPrincipal,
        UserManager<AppUser> userManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (!string.IsNullOrEmpty(infoRequest.NewPassword))
        {
            if (string.IsNullOrEmpty(infoRequest.OldPassword))
            {
                return CreateValidationProblem("OldPasswordRequired", "The old password is required to set a new password.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return CreateValidationProblem(changePasswordResult);
            }
        }

        return TypedResults.Ok();
    }
}
