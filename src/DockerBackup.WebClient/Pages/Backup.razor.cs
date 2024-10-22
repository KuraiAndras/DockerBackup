using DockerBackup.ApiClient;

using Microsoft.AspNetCore.Components;

namespace DockerBackup.WebClient.Pages;

public partial class Backup
{
    [Parameter] public required Guid ContainerId { get; init; }

    [Inject] public required IClient Client { get; init; }

    private List<ContainerBackupResponse>? _backups;

    override protected async Task OnInitializedAsync() => await Refresh();

    private async Task Refresh() => _backups = await Client.GetBackupsForContainerAsync(ContainerId);
}
