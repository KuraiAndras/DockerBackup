using Docker.DotNet;
using Docker.DotNet.Models;

using DockerBackup.ApiClient;
using DockerBackup.WebApi.Database;
using DockerBackup.WebApi.Extensions;
using DockerBackup.WebApi.Options;

using Microsoft.Extensions.Options;

namespace DockerBackup.WebApi.Controllers;

public sealed class ControllerImplementation
(
    IDockerClient _docker,
    ILogger<ControllerImplementation> _logger,
    TimeProvider _timeProvider,
    IOptions<BackupOptions> _backupOptions,
    ApplicationDb _db
)
    : IController
{
    public async Task<SwaggerResponse<CreateBackupRespone>> CreateBackupAsync(CreateBackupRequest body, CancellationToken cancellationToken = default)
    {
        var backupFilePaths = new List<string>();
        ContainerListResponse? container = null;
        try
        {
            var now = _timeProvider.GetLocalNow();

            var containers = await _docker.Containers.ListContainersAsync(new()
            {
                All = true,
            }, cancellationToken);

            container = containers.SingleOrDefault(c => c.Names.Contains(body.ContainerName));

            if (container is null)
            {
                _logger.LogWarning("Container {ContainerName} not found", body.ContainerName);
                return new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = $"Container {body.ContainerName} not found",
                    Detail = null,
                    Instance = null,
                    Type = null,
                    AdditionalProperties = null!,
                };
            }

            var containerPathsToBackUp = container.Mounts
                .Where(m => m.Type == "bind")
                .Where(m => body.Directories.Contains(m.Source))
                .Select(m => m.Destination)
                .ToArray();

            var backupDirectory = Path.Combine(_backupOptions.Value.BackupPath, body.ContainerName.TrimStart('/'), $"{now:yyyy-MM-ddTHH-mm-ss}");

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            if (container.State == "running")
            {
                await _docker.Containers.StopContainerAsync(container.ID, new()
                {
                    WaitBeforeKillSeconds = body.WaitForContainerStopMs.HasValue
                        ? (uint)body.WaitForContainerStopMs
                        : _backupOptions.Value.DefaultWaitForContainerStopMs,
                }, cancellationToken);
            }

            foreach (var containerPathToBackUp in containerPathsToBackUp)
            {
                await using DockerArchive archive = await _docker.Containers.GetArchiveFromContainerAsync(container.ID, new()
                {
                    Path = containerPathToBackUp,
                }, statOnly: false, cancellationToken);

                var tarFileName = Path.Combine(backupDirectory, $"{containerPathToBackUp.TrimStart('/').Replace("/", "__")}.tar");
                backupFilePaths.Add(tarFileName);

                await using var folderTarBall = File.Create(tarFileName);
                await archive.Stream.CopyToAsync(folderTarBall, cancellationToken);
            }

            var containerBackup = new ContainerBackup
            {
                ContainerName = body.ContainerName,
                CreatedAt = now,
                Files = containerPathsToBackUp.Select((containerPathToBackUp, i) => new FileBackup
                {
                    FilePath = backupFilePaths[i],
                    ContainerPath = containerPathToBackUp,
                }).ToList(),
            };
            _db.ContainerBackups.Add(containerBackup);

            await _db.SaveChangesAsync(cancellationToken);

            return new CreateBackupRespone
            {
                BackupId = containerBackup.Id,
            };
        }
        catch (Exception e)
        {
            foreach (var backupFilePath in backupFilePaths)
            {
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }
            }

            _logger.LogError(e, "Creating backup {ContainerName} failed", body.ContainerName);
            return new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Creating backup failed",
                Detail = null,
                Instance = null,
                Type = null,
                AdditionalProperties = null!,
            };
        }
        finally
        {
            if (container?.State == "running")
            {
                await _docker.Containers.StartContainerAsync(container.ID, new(), cancellationToken);
            }
        }
    }

    public async Task<SwaggerResponse<ICollection<Container>>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var containers = await _docker.Containers.ListContainersAsync(new()
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
                State = c.State,
                BackupDirectories = c.Mounts
                    .Where(m => m.Type == "bind")
                    .Select(m => m.Source)
                    .ToList(),
            })
            .ToArray();
    }
}
