using System.Text.Json.Serialization;

namespace DockerBackup.ApiClient;

public partial class Container
{
    [JsonIgnore]
    public string Name => Names.First();

    [JsonIgnore]
    public string ShortId => Id[..12];
}
