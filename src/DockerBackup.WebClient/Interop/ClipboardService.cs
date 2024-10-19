using Microsoft.JSInterop;

namespace DockerBackup.WebClient.Interop;

public interface IClipboardService
{
    ValueTask<string> ReadTextAsync();
    ValueTask WriteTextAsync(string text);
}

public sealed class ClipboardService(IJSRuntime _jsRuntime) : IClipboardService
{
    public async ValueTask<string> ReadTextAsync() =>
        await _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");

    public async ValueTask WriteTextAsync(string text) =>
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
}
