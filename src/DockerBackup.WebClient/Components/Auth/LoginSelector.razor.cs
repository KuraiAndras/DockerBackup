using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

namespace DockerBackup.WebClient.Components.Auth;
public partial class LoginSelector
{
    private bool? _areUsersSetUp;

    [Inject] public required IClient Client { get; init; }

    protected override async Task OnInitializedAsync()
    {
        var response = await Client.UserSetUpAsync();

        _areUsersSetUp = response.IsUserSetUp;
    }
}
