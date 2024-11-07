namespace DockerBackup.WebApi.Endpoints.Identity;

public interface IIdentityService
{
    Task<bool> CanUsersRegister(CancellationToken cancellationToken = default);
}
