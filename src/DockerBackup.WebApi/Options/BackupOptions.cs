namespace DockerBackup.WebApi.Options;

public sealed class BackupOptions
{
    public const string Section = "Backup";

    /// <summary>
    /// The full path where the backups are stored.
    /// </summary>
    public string BackupPath { get; set; } = "/backups";

    /// <summary>
    /// The default time in milliseconds to wait for a container to stop before stopping it.
    /// </summary>
    public uint DefaultWaitForContainerStopMs { get; set; } = 5000;

    /// <summary>
    /// The file name of the SQLite database.
    /// </summary>
    public string DatabaseFileName { get; set; } = "DockerBackup.db";

    /// <summary>
    /// The full path where all persistent data is stored except for backups.
    /// </summary>
    public string ConfigDirectoryPath { get; set; } = "/config";
}
