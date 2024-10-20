namespace DockerBackup.WebApi.Database;

public sealed class ContainerBackup
{
    public Guid Id { get; set; }
    public string ContainerName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<FileBackup> Files { get; set; } = default!;
}
