namespace DockerBackup.WebApi.Options;

public sealed class BackupOptions
{
    public const string Section = "Backup";

    /// <summary>
    /// The default time in milliseconds to wait for a container to stop before stopping it.
    /// </summary>
    public uint DefaultWaitForContainerStopMs { get; set; } = 5000;

    /// <summary>
    /// The default cron for the backup jobs.
    /// </summary>
    public string Cron { get; set; } = "0 4 * * *";

    /// <summary>
    /// Cron for checking containers for backup configurations.
    /// </summary>
    public string ContainerScanCron { get; set; } = "*/10 * * * *";
}
