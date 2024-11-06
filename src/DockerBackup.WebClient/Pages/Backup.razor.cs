using DockerBackup.ApiClient;
using DockerBackup.WebClient.Extensions;

using Microsoft.AspNetCore.Components;

namespace DockerBackup.WebClient.Pages;

public partial class Backup
{
    [Parameter] public required string ContainerName { get; init; }

    [Inject] public required IClient Client { get; init; }

    private List<ContainerBackupResponse> _backups = [];

    protected override async Task RefreshInternal() =>
        _backups = await Client.GetBackupsForContainerAsync(ContainerName);

    private async Task RestoreBackup(Guid backupId) =>
        await Snackbar.Run
        (
            async () => await Client.RestoreBackupAsync(backupId),
            "Restored backup",
            "Restoring backup failed"
        );
}
