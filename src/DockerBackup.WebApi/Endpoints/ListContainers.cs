using Docker.DotNet;

using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Http.HttpResults;

namespace DockerBackup.WebApi.Endpoints;

public static class ListContainers
{
    public static async Task<Ok<Container[]>> Handle(IDockerClient docker, CancellationToken cancellationToken = default)
    {
        var containers = await docker.Containers.ListContainersAsync(new()
        {
            All = true,
        }, cancellationToken);

        return TypedResults.Ok(containers
            .Select(c => new Container
            (
                Id: c.ID,
                Names: c.Names,
                Image: c.Image,
                Status: c.Status,
                State: c.State,
                BackupDirectories: c.Labels
                    .Where(l => l.Key.StartsWith("backup.dir."))
                    .Select(l => l.Value)
                    .ToList()
            ))
            .ToArray());
    }
}
