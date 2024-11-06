using System.Security.Claims;

using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components.Authorization;

namespace DockerBackup.WebClient.Auth;

public sealed class CookieAuthenticationStateProvider(IClient client) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal? user = null;

        try
        {
            var userInfo = await client.GetApiManageInfoAsync();
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userInfo.UserName)
            };
            var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
            user = new ClaimsPrincipal(id);
        }
        catch
        {
            user = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new(user);
    }

    public async Task Login(LoginRequest loginRequest)
    {
        await client.PostApiLoginAsync(loginRequest);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await client.PostApiLogoutAsync();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
