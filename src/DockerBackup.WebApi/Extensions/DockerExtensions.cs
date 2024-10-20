
using Docker.DotNet.Models;

namespace DockerBackup.WebApi.Extensions;

public sealed record DockerArchive(ContainerPathStatResponse Stat, Stream Stream) : IAsyncDisposable
{
    public async ValueTask DisposeAsync() => await Stream.DisposeAsync();

    public static implicit operator DockerArchive(GetArchiveFromContainerResponse response) => new(response.Stat, response.Stream);
}
