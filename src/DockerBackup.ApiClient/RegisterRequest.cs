namespace DockerBackup.ApiClient;

/// <summary>
/// The request type for the "/register" endpoint.
/// </summary>
public sealed class RegisterRequest
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
