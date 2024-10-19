using Docker.DotNet;
using Docker.DotNet.Models;

using DockerBackup.ApiClient;

namespace DockerBackup.WebApi.Controllers;

public sealed class ControllerImplementation(IDockerClient _docker) : IController
{
    public async Task<SwaggerResponse<ICollection<Container>>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters()
        {
            All = true,
        }, cancellationToken);

        return containers
            .Select(c => new Container
            {
                Id = c.ID,
                Names = c.Names,
                Image = c.Image,
                Status = c.Status,
                BackupDirectories = c.Mounts
                    .Where(m => m.Type == "bind")
                    .Select(m => $"{m.Source}:{m.Destination}")
                    .ToList(),
            })
            .ToArray();
    }
}
