using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace DockerBackup.WebApi.Endpoints.Identity;

public static class Logout
{
    public static async Task<EmptyHttpResult> Handle(SignInManager<AppUser> signInManager)
    {
        await signInManager.SignOutAsync();

        return TypedResults.Empty;
    }
}
