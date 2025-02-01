using DockerBackup.ApiClient;
using DockerBackup.WebClient.Components;
using DockerBackup.WebClient.Extensions;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Backup
{
    [Parameter] public required string ContainerName { get; init; }

    [Inject] public required IClient Client { get; init; }
    [Inject] public required IDialogService DialogService { get; init; }

    private List<ContainerBackupResponse> _backups = [];

    protected override async Task RefreshInternal() =>
        _backups = await Client.GetBackupsForContainerAsync(ContainerName);

    private async Task RestoreBackup(ContainerBackupResponse backup) =>
        await Snackbar.Run
        (
            async () => await Client.RestoreBackupAsync(backup.Id),
            "Restored backup",
            "Restoring backup failed"
        );

    private async Task DeleteBackup(ContainerBackupResponse backup)
    {
        var parameters = new DialogParameters<DeleteDialog>
        {
            { x => x.ContentText, $"Do you want to delete the backup of {ContainerName} from {backup.CreatedAt.ToLocalTime()}?"}
        };

        var dialog = await DialogService.ShowAsync<DeleteDialog>("Delete Backup", parameters);

        var result = await dialog.Result;

        if (result?.Data is true)
        {
            await Snackbar.Run
            (
                async () => await Client.DeleteBackupAsync(backup.Id),
                "Deleted backup",
                "Deleting backup failed"
            );

            await Refresh();
        }
    }
}
