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
