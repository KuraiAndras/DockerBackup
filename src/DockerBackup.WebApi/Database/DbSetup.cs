using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Database;

public interface IDbSetup
{
    ValueTask Setup(CancellationToken cancellationToken = default);
}

public sealed class DbSetup(ApplicationDb _db) : IDbSetup
{
    public async ValueTask Setup(CancellationToken cancellationToken = default) =>
        await _db.Database.MigrateAsync(cancellationToken);
}
