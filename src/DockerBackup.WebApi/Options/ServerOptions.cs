namespace DockerBackup.WebApi.Options;

public sealed class ServerOptions
{
    public const string Section = "Sever";

    /// <summary>
    /// The full path where the backups are stored.
    /// </summary>
    public string BackupPath { get; set; } = "/backups";

    /// <summary>
    /// The file name of the SQLite database.
    /// </summary>
    public string DatabaseFileName { get; set; } = "DockerBackup.db";

    /// <summary>
    /// The file name of the Hangfire SQLite database.
    /// </summary>
    public string HangfireDatabaseFileName { get; set; } = "hangfire.db";

    /// <summary>
    /// The full path where all persistent data is stored except for backups.
    /// </summary>
    public string ConfigDirectoryPath { get; set; } = "/config";

    /// <summary>
    /// Whether to log in JSON format.
    /// </summary>
    public bool LogInJson { get; set; }

    public string DatabaseFilePath() => Path.Combine(ConfigDirectoryPath, DatabaseFileName);

    public string HangfireDatabaseFilePath() => Path.Combine(ConfigDirectoryPath, HangfireDatabaseFileName);
}
