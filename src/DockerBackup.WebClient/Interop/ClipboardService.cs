using Microsoft.JSInterop;

namespace DockerBackup.WebClient.Interop;

public interface IClipboardService
{
    ValueTask<string> ReadTextAsync();
    ValueTask WriteTextAsync(string text);
}

public sealed class ClipboardService(IJSRuntime jsRuntime) : IClipboardService
{
    public async ValueTask<string> ReadTextAsync() =>
        await jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");

    public async ValueTask WriteTextAsync(string text) =>
        await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
}
