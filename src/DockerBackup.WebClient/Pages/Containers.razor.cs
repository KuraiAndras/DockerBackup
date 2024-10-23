using DockerBackup.ApiClient;
using DockerBackup.WebClient.Components;
using DockerBackup.WebClient.Extensions;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Pages;

public partial class Containers
{
    private readonly Dictionary<string, List<string>> _selectedBackups = [];

    private List<Container> _containers = [];

    [Inject] public required IClient Client { get; init; }

    private bool SaveAllDisabled => !_selectedBackups.SelectMany(x => x.Value).Any();

    protected override async Task RefreshInternal()
    {
        _containers = await Client.GetContainersAsync();
        _selectedBackups.Clear();

        foreach (var container in _containers)
        {
            _selectedBackups[container.Name] = [];
        }
    }

    private void OnBackupSelectionChanged(BackupSelectionList.SelectionChangedParams parameters) =>
        _selectedBackups[parameters.Container.Name] = [.. parameters.SelectedBackups];

    private void SaveAll() => Snackbar.Add("Not yet implemented", Severity.Warning);

    private async Task Save(Container container)
    {
        await Snackbar.Run(
        async () => await Client.CreateBackupAsync(new CreateBackupRequest
        (
            ContainerName: container.Name,
            Directories: _selectedBackups[container.Name] is { Count: > 0 } selectedBackups
                ? selectedBackups
                : container.BackupDirectories,
            WaitForContainerStopMs: null
        )), $"Backing up {container.Name} successful", $"Backing up {container.Name} failed");
    }
}
