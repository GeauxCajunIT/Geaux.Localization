using Microsoft.JSInterop;

namespace Geaux.Localization.SampleBlazor.Services;

public sealed class DownloadService(IJSRuntime js)
{
    public async Task DownloadAsync(string filename, string contentType, byte[] bytes)
    {
        string base64 = Convert.ToBase64String(bytes);
        await js.InvokeVoidAsync("geauxDownload", filename, contentType, base64);
    }
}
