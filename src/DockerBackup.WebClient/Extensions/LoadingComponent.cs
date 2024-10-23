using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace DockerBackup.WebClient.Extensions;

public abstract class LoadingComponent : ComponentBase
{
    protected virtual bool Loading { get; set; }

    protected virtual string? LoadingErrorMessage { get; }

    [Inject] public required ISnackbar Snackbar { get; init; }

    protected override async Task OnInitializedAsync() => await Refresh();

    protected abstract Task RefreshInternal();

    protected virtual async Task Refresh()
    {
        Loading = true;
        try
        {
            await RefreshInternal();
        }
        catch
        {
            Snackbar.Add(LoadingErrorMessage ?? "Loading failed", Severity.Error);
        }
        finally
        {
            Loading = false;
        }
    }
}
