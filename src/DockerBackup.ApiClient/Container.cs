using System.Text.Json.Serialization;

namespace DockerBackup.ApiClient;

public record Container(
    string Id,
    ICollection<string> Names,
    string Status,
    string State,
    string Image,
    ICollection<string> BackupDirectories
)
{
    [JsonIgnore]
    public string Name => Names.First();

    [JsonIgnore]
    public string ShortId => Id[..12];
}

public record ListContainerBackupResponse(Guid Id, string ContainerName, DateTime? LastBackupAt, int? NumberOfBackups, long? SizeInBytes);

public record ContainerBackupResponse(Guid Id, DateTime CreatedAt, FileBackupResponse[] Files)
{
    [JsonIgnore]
    public long SizeInBytes => Files.Sum(f => f.SizeInBytes);
}

public record FileBackupResponse(Guid Id, string FilePath, string ContainerPath, long SizeInBytes);
