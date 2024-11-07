using DockerBackup.WebApi.Database;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DockerBackup.WebApi.Endpoints.Identity;

public sealed class IdentityService(UserManager<AppUser> userManager) : IIdentityService
{
    public async Task<bool> CanUsersRegister(CancellationToken cancellationToken) => await userManager.Users.AnyAsync(cancellationToken);
}
