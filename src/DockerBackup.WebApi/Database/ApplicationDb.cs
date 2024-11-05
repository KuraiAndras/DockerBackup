using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DockerBackup.WebApi.Database;

public sealed class ApplicationDb(DbContextOptions<ApplicationDb> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<ContainerBackup> ContainerBackups { get; set; } = default!;
    public DbSet<FileBackup> FileBackups { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<decimal>()
            .HaveConversion<double>();

        configurationBuilder
            .Properties<TimeSpan>()
            .HaveConversion<TimeSpanToTicksConverter>();
    }
}
