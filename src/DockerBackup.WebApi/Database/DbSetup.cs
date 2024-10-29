using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Database;

public sealed class DbSetup(ApplicationDb db)
{
    public async ValueTask Setup(CancellationToken cancellationToken = default) =>
        await db.Database.MigrateAsync(cancellationToken);
}
