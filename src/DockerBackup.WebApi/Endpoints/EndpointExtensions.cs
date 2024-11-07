namespace DockerBackup.WebApi.Endpoints;

public static class EndpointExtensions
{
    public static RouteHandlerBuilder WithOpenApi(this RouteHandlerBuilder route, string group, string name) =>
        route
            .WithName(name)
            .WithTags(group)
            .WithOpenApi();
}
