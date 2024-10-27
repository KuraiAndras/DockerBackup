namespace DockerBackup.ApiClient;

public record CreateBackupRequest(string ContainerName, ICollection<string>? Directories, int? WaitForContainerStopMs);
