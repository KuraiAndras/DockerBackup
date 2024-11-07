using DockerBackup.WebApi.Endpoints;

using Microsoft.AspNetCore.Identity;

namespace DockerBackup.WebApi.Endpoints.Identity;

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

        var routeGroup = endpoints.MapGroup("identity");

        routeGroup
            .MapPost("/register", Register.Handle)
            .WithName("register")
            .WithTags("identity");

        routeGroup
            .MapPost("/login", Login.Handle)
            .WithOpenApi("identity", "login");

        routeGroup
            .MapPost("/logout", Logout.Handle)
            .WithOpenApi("identity", "logout");

        var accountGroup = routeGroup
            .MapGroup("/manage")
            .RequireAuthorization();

        accountGroup
            .MapGet("/info", Info.Handle)
            .WithOpenApi("manage", "info");

        accountGroup
            .MapPost("/changePassword", ChangePassword.Handle)
            .WithOpenApi("manage", "change-password");

        accountGroup
            .MapGet("/user-set-up", UserSetUp.Handle)
            .WithOpenApi("manage", "user-set-up");
    }
}
