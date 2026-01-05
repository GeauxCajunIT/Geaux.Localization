using Bunit;
using Geaux.Localization.Admin.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Geaux.Localization.Admin.Tests;

public abstract class AdminTestContext : TestContext, IAsyncDisposable
{
    protected AdminTestContext()
    {
        // MudBlazor DI
        Services.AddMudServices();

        // JSInterop mocks for MudBlazor popovers/dialogs/snackbars
        JSInterop.SetupVoid("mudPopover.initialize", _ => true);
        JSInterop.SetupVoid("mudPopover.connect", _ => true);
        JSInterop.SetupVoid("mudPopover.disconnect", _ => true);
        JSInterop.SetupVoid("mudPopover.update", _ => true);

        // Admin services
        Services.AddScoped<KeyAdminService>();
        Services.AddScoped<CultureAdminService>();
        Services.AddScoped<LanguagePackAdminService>();

        // Add MudBlazor providers to the render tree
        RenderTree.Add<MudThemeProvider>();
        RenderTree.Add<MudPopoverProvider>();
        RenderTree.Add<MudDialogProvider>();
        RenderTree.Add<MudSnackbarProvider>();
    }

    public async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}
