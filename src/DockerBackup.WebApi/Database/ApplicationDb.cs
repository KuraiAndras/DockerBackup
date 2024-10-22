using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DockerBackup.WebApi.Database;

public sealed class ApplicationDb(DbContextOptions<ApplicationDb> options) : DbContext(options)
{
    public DbSet<ContainerBackup> ContainerBackups { get; set; } = default!;
    public DbSet<FileBackup> FileBackups { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContainerBackup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContainerName).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasMany(e => e.Files).WithOne(e => e.Backup).HasForeignKey(e => e.ContainerBackupId);
        });

        modelBuilder.Entity<FileBackup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).IsRequired();
            entity.Property(e => e.ContainerPath).IsRequired();
            entity.HasOne(e => e.Backup).WithMany(e => e.Files).HasForeignKey(e => e.ContainerBackupId);
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        if (!Database.IsSqlite()) return;

        configurationBuilder
            .Properties<decimal>()
            .HaveConversion<double>();

        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToStringConverter>();

        configurationBuilder
            .Properties<TimeSpan>()
            .HaveConversion<TimeSpanToStringConverter>();
    }
}
