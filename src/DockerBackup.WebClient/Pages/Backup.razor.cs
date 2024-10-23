using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Backup
{
    [Parameter] public required string ContainerName { get; init; }

    [Inject] public required IClient Client { get; init; }

    private List<ContainerBackupResponse> _backups = [];

    protected override async Task RefreshInternal() =>
        _backups = await Client.GetBackupsForContainerAsync(ContainerName);

    private void RestoreBackup(Guid backupId) => Snackbar.Add($"{backupId}, Not yet implemented", Severity.Error);
}
