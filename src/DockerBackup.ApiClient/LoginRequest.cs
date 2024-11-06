namespace DockerBackup.ApiClient;

/// <summary>
/// The request type for the "/login" endpoint.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// The user's user name.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}
