using System.Diagnostics.CodeAnalysis;

using DockerBackup.ApiClient;
using DockerBackup.WebClient.Auth;
using DockerBackup.WebClient.Extensions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;

namespace DockerBackup.WebClient.Components;
public partial class Login
{
    private class Model
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        // TODO: fluent validation
        [MemberNotNullWhen(true, nameof(UserName), nameof(Password))]
        public bool IsValid() => UserName is not null && Password is not null;
    }

    private readonly Model _model = new();

    [Inject] public required IClient Client { get; init; }
    [Inject] public required ISnackbar Snackbar { get; init; }
    [Inject] public required CookieAuthenticationStateProvider Auth { get; init; }

    private async Task OnSubmit(EditContext editContext)
    {
        if (!_model.IsValid())
        {
            Snackbar.Add("Login details are not valid", Severity.Error);
            return;
        }

        await Snackbar.Run(async () =>
            await Auth.Login(new LoginRequest()
            {
                UserName = _model.UserName,
                Password = _model.Password,
            }),
            "Login successful",
            "Login failed");
    }
}
