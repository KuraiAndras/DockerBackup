using System.Globalization;
using System.IO.Compression;

using Docker.DotNet.Models;

using DockerBackup.WebApi.Options;

namespace DockerBackup.WebApi.Domain;

public sealed class ContainerBackupInfo
{
    public static class Keys
    {
        public const string Directory = "backup.dir";
        public const string Cron = "backup.cron";
        public const string MaximumBackups = "backup.maximum-backups";
        public const string WaitBeforeKill = "backup.wait";
        public const string Compress = "backup.compress";
        public const string CompressionMode = "backup.compression-mode";
    }

    public string Id { get; }

    public string Name { get; }

    public IReadOnlyList<string> Directories { get; }

    public bool NeedsBackup => Directories.Count != 0;

    public string Cron { get; }

    public int? MaximumBackups { get; }

    public bool IsRunning { get; }

    public bool Compress { get; }

    public CompressionLevel CompressionMode { get; }

    /// <summary>
    /// The time to wait before kill the container when stopping in seconds
    /// </summary>
    public uint? WaitBeforeKill { get; }

    public ContainerBackupInfo(ContainerListResponse container, BackupOptions options)
        : this(container.ID, container.Names, container.Labels, container.Status, options)
    {
    }

    public ContainerBackupInfo(ContainerInspectResponse container, BackupOptions options)
        : this(container.ID, [container.Name], container.Config.Labels, container.State.Status, options)
    {
    }

    public ContainerBackupInfo(string id, IEnumerable<string> names, IDictionary<string, string> labels, string status, BackupOptions options)
    {
        Id = id;
        Name = names.First();
        Directories = [.. GetBackupPaths(labels)];
        Cron = GetBackupCron(labels) ?? options.Cron;
        MaximumBackups = GetMaximumBackups(labels) ?? options.MaximumBackups;
        IsRunning = status == "running";
        WaitBeforeKill = GetWaitBeforeKill(labels) ?? options.WaitBeforeKill;
        Compress = GetCompress(labels) ?? options.Compress;
        CompressionMode = GetCompressionMode(labels) ?? options.CompressionMode;
    }

    public string GetBackupDirectory(ServerOptions options, DateTime now) =>
        Path.Combine(options.BackupPath, Name.TrimStart('/'), $"{now:yyyy-MM-ddTHH-mm-ss}");

    public static string GetBackupFileName(string backupDirectory, string containerPathToBackUp) =>
        Path.Combine(backupDirectory, $"{containerPathToBackUp.TrimStart('/').Replace("/", "__")}.tar");

    public static IEnumerable<string> GetBackupPaths(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key.StartsWith(Keys.Directory))
            .Select(l => l.Value);

    public static string? GetBackupCron(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key == Keys.Cron)
            .Select(l => l.Value)
            .SingleOrDefault();

    public static int? GetMaximumBackups(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key == Keys.MaximumBackups)
            .Select(l => int.TryParse(l.Value, CultureInfo.InvariantCulture, out var count) ? count : null as int?)
            .SingleOrDefault();

    public static uint? GetWaitBeforeKill(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key == Keys.WaitBeforeKill)
            .Select(l => uint.TryParse(l.Value, CultureInfo.InvariantCulture, out var count) ? count : null as uint?)
            .SingleOrDefault();

    public static bool? GetCompress(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key == Keys.Compress)
            .Select(l => bool.TryParse(l.Value, out var compress) ? compress : null as bool?)
            .SingleOrDefault();


    public static CompressionLevel? GetCompressionMode(IDictionary<string, string> labels) =>
        labels
            .Where(l => l.Key == Keys.CompressionMode)
            .Select(l => Enum.TryParse<CompressionLevel>(l.Value, out var compressionMode) ? compressionMode : null as CompressionLevel?)
            .SingleOrDefault();
}
