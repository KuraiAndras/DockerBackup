using DockerBackup.WebApi.Endpoints.Identity;

namespace DockerBackup.WebApi.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapDockerBackup(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api");

        api.MapIdentity();

        api
            .MapGet("version", GetVersion.Handle)
            .WithOpenApi("api", "get-version");

        var containers = api
            .MapGroup("containers")
            .RequireAuthorization();

        containers
            .MapGet(ListContainers.Handle)
            .WithOpenApi("containers", "get-containers");

        containers
            .MapPost("create-backup", CreateBackup.Handle)
            .WithOpenApi("containers", "create-backup");

        var backups = api
            .MapGroup("backups")
            .RequireAuthorization();

        backups
            .MapGet(ListBackups.Handle)
            .WithOpenApi("backups", "get-backups");

        backups
            .MapGet("{containerName}", GetBackupsForContainer.Handle)
            .WithOpenApi("backups", "get-backups-for-container");

        backups
            .MapPost("{backupId}/restore", RestoreContainer.Handle)
            .WithOpenApi("backups", "restore-backup");

        backups
            .MapGet("overall-storage-size", GetOverallBackupStorageSize.Handle)
            .WithOpenApi("backups", "get-backup-overall-storage-size");

        return app;
    }

    private static RouteHandlerBuilder MapGet(this RouteGroupBuilder endpoints, Delegate handler) =>
        endpoints.MapGet("", handler);
}
