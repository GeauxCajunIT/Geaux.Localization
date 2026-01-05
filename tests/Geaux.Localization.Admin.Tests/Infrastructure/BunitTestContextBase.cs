using Bunit;
using MudBlazor.Services;

namespace Geaux.Localization.Admin.Tests.Infrastructure;

public abstract class BunitTestContextBase : BunitContext
{
    protected BunitTestContextBase()
    {
        // Your own extension that wires up DbContext, services, etc.
        //Services.AddAdminTestServices();

        // MudBlazor services
        Services.AddMudServices();

        // MudBlazor JSInterop expectations
        JSInterop.SetupVoid("mudPopover.initialize", _ => true);
        JSInterop.SetupVoid("mudPopover.connect", _ => true);
        JSInterop.SetupVoid("mudPopover.disconnect", _ => true);
        JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
        JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
        JSInterop.SetupVoid("mudKeyInterceptor.disconnect", _ => true);

        // Your RCL’s JS module
        JSInterop.SetupModule("./_content/Geaux.Localization.Admin/js/fileDrop.js");
    }
}
