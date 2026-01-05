using Microsoft.JSInterop;

namespace Geaux.Shared.UI;

public sealed class ThemeService
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "geaux-theme-dark";

    public bool IsDarkMode { get; private set; }

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        string value = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);

        if (bool.TryParse(value, out bool dark))
            IsDarkMode = dark;
        else
            IsDarkMode = false;
    }

    public async Task SetDarkMode(bool dark)
    {
        IsDarkMode = dark;
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, dark.ToString().ToLower());
    }
}
