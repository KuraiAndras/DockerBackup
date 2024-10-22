namespace DockerBackup.WebApi.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapDockerBackup(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api");

        var containers = api.MapGroup("containers");

        containers.MapGet(ListContainers.Handle);
        containers.MapPost("create-backup", CreateBackup.Handle);

        return app;
    }

    public static RouteHandlerBuilder MapGet(this RouteGroupBuilder endpoints, Delegate handler) =>
        endpoints.MapGet("", handler);
}
