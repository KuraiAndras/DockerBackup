using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Backups
{
    [Inject] public required IClient Client { get; init; }
    [Inject] public required NavigationManager Navigation { get; init; }

    private List<ListContainerBackupResponse>? _backups;

    protected override async Task OnInitializedAsync() => await Refresh();

    private async Task Refresh() => _backups = await Client.GetBackupsAsync();

    private void BackupClicked(TableRowClickEventArgs<ListContainerBackupResponse> tableRowClickEventArgs) =>
        Navigation.NavigateTo(Backup.PageUri(tableRowClickEventArgs.Item!.Id));
}
