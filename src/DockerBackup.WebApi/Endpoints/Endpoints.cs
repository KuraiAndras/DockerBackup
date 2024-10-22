namespace DockerBackup.WebApi.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapDockerBackup(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api");

        var containers = api.MapGroup("containers");

        containers.MapGet(ListContainers.Handle);
        containers.MapPost("create-backup", CreateBackup.Handle);

        var backups = api.MapGroup("backups");

        backups.MapGet(ListBackups.Handle);
        backups.MapGet("{containerId}", GetBackupsForContainer.Handle);

        return app;
    }

    public static RouteHandlerBuilder MapGet(this RouteGroupBuilder endpoints, Delegate handler) =>
        endpoints.MapGet("", handler);
}
