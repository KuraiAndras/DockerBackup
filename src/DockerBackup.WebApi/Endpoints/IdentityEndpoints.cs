using System.Diagnostics;
using System.Security.Claims;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DockerBackup.WebApi.Endpoints;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add identity endpoints.
/// </summary>
public static class IdentityEndpoints
{
    /// <summary>
    /// Add endpoints for registering, logging in, and logging out using ASP.NET Core Identity.
    /// </summary>
    /// <typeparam name="TUser">The type describing the user. This should match the generic parameter in <see cref="UserManager{TUser}"/>.</typeparam>
    /// <param name="endpoints">
    /// The <see cref="IEndpointRouteBuilder"/> to add the identity endpoints to.
    /// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
    /// </param>
    public static void MapIdentity(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var routeGroup = endpoints.MapGroup("");

        routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
            ([FromBody] RegisterRequest registration, UserManager<AppUser> userManager, IUserStore<AppUser> userStore, CancellationToken cancellationToken) =>
        {
            var result = await userManager.CreateAsync(
                new AppUser()
                {
                    UserName = registration.UserName
                },
                registration.Password);

            return result.Succeeded
                ? TypedResults.Ok()
                : CreateValidationProblem(result);
        });

        routeGroup.MapPost("/login", async Task<Results<EmptyHttpResult, ProblemHttpResult>>
            ([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, SignInManager<AppUser> signInManager) =>
        {
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent, lockoutOnFailure: true);

            return result.Succeeded
                ? TypedResults.Empty // The signInManager already produced the needed response in the form of a cookie.
                : TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        });

        routeGroup.MapPost("/logout", async Task<EmptyHttpResult> (SignInManager<AppUser> signInManager) =>
        {
            await signInManager.SignOutAsync();

            return TypedResults.Empty;
        });

        var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();

        accountGroup.MapGet("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, UserManager<AppUser> userManager) =>
                await userManager.GetUserAsync(claimsPrincipal) is { } user
                    ? TypedResults.Ok(await CreateInfoResponseAsync(user, userManager))
                    : TypedResults.NotFound());

        accountGroup.MapPost("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
            ([FromBody] InfoRequest infoRequest, ClaimsPrincipal claimsPrincipal, UserManager<AppUser> userManager) =>
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

            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        });
    }

    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            { errorCode, [errorDescription] },
        });

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync(AppUser user, UserManager<AppUser> userManager)
    {
        return new()
        {
            UserName = await userManager.GetUserNameAsync(user) ?? throw new NotSupportedException("Users must have a name."),
        };
    }
}
