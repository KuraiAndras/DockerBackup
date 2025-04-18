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
            var userInfo = await client.InfoAsync();
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

    public async Task Login(LoginRequest request)
    {
        await client.LoginAsync(request);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await client.LogoutAsync();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Register(RegisterRequest request)
    {
        await client.RegisterAsync(request);

        await Login(new LoginRequest
        {
            UserName = request.UserName,
            Password = request.Password,
        });
    }
}
