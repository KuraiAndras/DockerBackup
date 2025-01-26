using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Backups
{
    [Inject] public required IClient Client { get; init; }
    [Inject] public required NavigationManager Navigation { get; init; }

    public List<ListContainerBackupResponse> ContainerBackups { get; private set; } = [];

    public long? OverallSizeInBytes { get; private set; }

    protected override async Task RefreshInternal()
    {
        async Task GetContainers() => ContainerBackups = await Client.GetBackupsAsync();
        async Task GetSizeInBytes() => OverallSizeInBytes = (await Client.GetBackupOverallStorageSizeAsync()).SizeInBytes;

        await Task.WhenAll([GetContainers(), GetSizeInBytes()]);
    }

    private void BackupClicked(TableRowClickEventArgs<ListContainerBackupResponse> tableRowClickEventArgs) =>
        Navigation.NavigateTo(Backup.PageUri(tableRowClickEventArgs.Item!.ContainerName));
}
