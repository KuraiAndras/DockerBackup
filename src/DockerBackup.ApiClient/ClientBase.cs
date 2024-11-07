using System.Text.Json;

namespace DockerBackup.ApiClient;

public abstract partial class ClientBase
{
    protected static void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.WriteIndented = false;
        settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        settings.PropertyNameCaseInsensitive = true;
    }
}
