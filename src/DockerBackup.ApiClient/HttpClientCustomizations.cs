using System.Text.Json;

namespace DockerBackup.ApiClient;

partial class Client
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.WriteIndented = false;
        settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        settings.PropertyNameCaseInsensitive = true;
    }
}
