using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Backup
{
    [Parameter] public required string ContainerName { get; init; }

    [Inject] public required IClient Client { get; init; }
    [Inject] public required ISnackbar Snackbar { get; init; }

    private List<ContainerBackupResponse> _backups = [];
    private bool _loading = true;

    override protected async Task OnInitializedAsync() => await Refresh();

    private async Task Refresh()
    {
        _loading = true;
        _backups = await Client.GetBackupsForContainerAsync(ContainerName);
        _loading = false;
    }

    private void RestoreBackup(Guid backupId) => Snackbar.Add($"{backupId}, Not yet implemented", Severity.Error);
}
