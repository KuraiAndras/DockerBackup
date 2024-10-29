namespace DockerBackup.WebApi.Jobs;

public record BackupJobParameters(string ContainerName, List<string>? Directories = null);
