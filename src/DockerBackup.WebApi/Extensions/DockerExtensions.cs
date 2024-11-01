using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerBackup.WebApi.Extensions;

public static class DockerExtensions
{
    public static class Keys
    {
        public const string Directory = "backup.dir";
        public const string Cron = "backup.cron";
    }

    public static bool NeedsBackup(this ContainerListResponse container) =>
        container.Labels.Any(l => l.Key.StartsWith(Keys.Directory));

    public static IEnumerable<string> GetBackupPaths(this Config config) =>
        config.Labels.GetBackupPaths();

    public static IEnumerable<string> GetBackupPaths(this ContainerListResponse container) =>
        container.Labels.GetBackupPaths();

    public static IEnumerable<string> GetBackupPaths(this IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key.StartsWith(Keys.Directory))
            .Select(l => l.Value);

    public static string? GetBackupCron(this ContainerListResponse container) =>
        container.Labels
            .Where(l => l.Key == Keys.Cron)
            .Select(l => l.Value)
            .SingleOrDefault();

    /// <summary>
    /// Retrieves low-level information about a container.
    /// </summary>
    /// <param name="id">The ID or name of the container.</param>
    /// <param name="cancellationToken">When triggered, the operation will stop at the next available time, if possible.</param>
    /// <returns>A <see cref="Task{TResult}"/> that resolves to a <see cref="ContainerInspectResponse"/>, which holds details about the container.</returns>
    /// <remarks>The corresponding commands in the Docker CLI are <c>docker inspect</c> and <c>docker container inspect</c>.</remarks>
    /// <exception cref="ArgumentNullException">One or more of the inputs was <see langword="null"/>.</exception>
    /// <exception cref="DockerApiException">the daemon experienced an error.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public static async Task<ContainerInspectResponse?> InspectContainerSafeAsync(this IContainerOperations containers, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await containers.InspectContainerAsync(id, cancellationToken);
        }
        catch (DockerContainerNotFoundException)
        {
            return null;
        }
    }
}
