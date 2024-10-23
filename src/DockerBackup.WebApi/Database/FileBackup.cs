namespace DockerBackup.WebApi.Database;

public sealed class FileBackup
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = default!;
    public string ContainerPath { get; set; } = default!;
    public Guid ContainerBackupId { get; set; }
    public ContainerBackup Backup { get; set; } = default!;
}
