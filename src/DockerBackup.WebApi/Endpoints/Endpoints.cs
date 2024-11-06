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
            .WithName("get-containers");

        containers
            .MapPost("create-backup", CreateBackup.Handle)
            .WithName("create-backup");

        var backups = api.MapGroup("backups");

        backups
            .MapGet(ListBackups.Handle)
            .WithName("get-backups");

        backups
            .MapGet("{containerName}", GetBackupsForContainer.Handle)
            .WithName("get-backups-for-container");

        backups
            .MapPost("{backupId}/restore", RestoreContainer.Handle)
            .WithName("restore-backup");

        return app;
    }

    public static RouteHandlerBuilder MapGet(this RouteGroupBuilder endpoints, Delegate handler) =>
        endpoints.MapGet("", handler);
}
