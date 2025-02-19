namespace DockerBackup.WebApi.Domain;

public interface IContainerBackupService
{
    Task CreateBackup(string containerName, List<string>? directories, CancellationToken cancellationToken = default);
}
