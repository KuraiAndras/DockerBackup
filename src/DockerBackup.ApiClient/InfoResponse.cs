namespace DockerBackup.ApiClient;

/// <summary>
/// The response type for the "/manage/info" endpoints.
/// </summary>
public sealed class InfoResponse
{
    /// <summary>
    /// The user name associated with the authenticated user.
    /// </summary>
    public required string UserName { get; init; }
}
