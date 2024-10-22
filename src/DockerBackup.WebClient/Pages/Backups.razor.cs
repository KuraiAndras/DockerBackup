using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

namespace DockerBackup.WebClient.Pages;

public partial class Backups
{
    [Inject] public required IClient Client { get; init; }

    private List<ListContainerBackupResponse>? _backups;

    protected override async Task OnInitializedAsync() => await Refresh();

    private async Task Refresh() => _backups = await Client.GetApiBackupsAllAsync();
}
