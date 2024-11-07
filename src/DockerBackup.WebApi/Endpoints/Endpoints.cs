using DockerBackup.WebApi.Endpoints.Identity;

namespace DockerBackup.WebApi.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapDockerBackup(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api");

        api.MapIdentity();

        var containers = api.MapGroup("containers");

        containers
            .MapGet(ListContainers.Handle)
            .WithOpenApi("containers", "get-containers");

        containers
            .MapPost("create-backup", CreateBackup.Handle)
            .WithOpenApi("containers", "create-backup");

        var backups = api.MapGroup("backups");

        backups
            .MapGet(ListBackups.Handle)
            .WithOpenApi("backups", "get-backups");

        backups
            .MapGet("{containerName}", GetBackupsForContainer.Handle)
            .WithOpenApi("backups", "get-backups-for-container");

        backups
            .MapPost("{backupId}/restore", RestoreContainer.Handle)
            .WithOpenApi("backups", "restore-backup");

        return app;
    }

    private static RouteHandlerBuilder MapGet(this RouteGroupBuilder endpoints, Delegate handler) =>
        endpoints.MapGet("", handler);
}
