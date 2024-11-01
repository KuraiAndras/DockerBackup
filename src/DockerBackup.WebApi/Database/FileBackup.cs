namespace DockerBackup.WebApi.Database;

public sealed class FileBackup
{
    [Obsolete("Use the factory methods for creating the entity")]
#pragma warning disable IDE0290 // Use primary constructor
    public FileBackup() { }
#pragma warning restore IDE0290 // Use primary constructor

    public Guid Id { get; set; }

    public string FilePath { get; set; } = default!;
    public string ContainerPath { get; set; } = default!;
    public long SizeInBytes { get; set; }

    public Guid ContainerBackupId { get; set; }
    public ContainerBackup Backup { get; set; } = default!;

#pragma warning disable CS0618 // Type or member is obsolete
    public static FileBackup Create(string filePath, string containerPath, long sizeInBytes) =>
        new()
        {
            FilePath = filePath,
            ContainerPath = containerPath,
            SizeInBytes = sizeInBytes,
        };
#pragma warning restore CS0618 // Type or member is obsolete
}
