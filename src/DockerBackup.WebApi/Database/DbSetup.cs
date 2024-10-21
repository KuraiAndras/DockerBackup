using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Database;

public interface IDbSetup
{
    ValueTask Setup(CancellationToken cancellationToken = default);
}

public sealed class DbSetup(ApplicationDb db) : IDbSetup
{
    public async ValueTask Setup(CancellationToken cancellationToken = default) =>
        await db.Database.MigrateAsync(cancellationToken);
}
