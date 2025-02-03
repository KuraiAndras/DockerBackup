using System.IO.Compression;

namespace DockerBackup.WebApi.Options;

public sealed class BackupOptions
{
    public const string Section = "Backup";

    /// <summary>
    /// The default time in seconds to wait for a container to stop before killing it.
    /// </summary>
    public uint? WaitBeforeKill { get; set; }

    /// <summary>
    /// The default cron for the backup jobs.
    /// </summary>
    public string Cron { get; set; } = "0 4 * * *";

    /// <summary>
    /// Cron for checking containers for backup configurations.
    /// </summary>
    public string ContainerScanCron { get; set; } = "*/10 * * * *";

    /// <summary>
    /// The maximum number of backups to keep per container. If unset, then the count is not limited.
    /// </summary>
    public int? MaximumBackups { get; set; }

    /// <summary>
    /// Run container scans on startup.
    /// </summary>
    public bool ScanContainersOnStartup { get; set; } = true;

    /// <summary>
    /// Compress the backup by default.
    /// </summary>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// Compression mode by default. Only applied if compression is turned on.
    /// </summary>
    public CompressionLevel CompressionMode { get; set; } = CompressionLevel.SmallestSize;
}
