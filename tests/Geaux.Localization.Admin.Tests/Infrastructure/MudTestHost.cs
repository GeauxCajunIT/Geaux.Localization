using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

namespace Geaux.Localization.Admin.Tests.Infrastructure;

public class MudTestHost : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudPopoverProvider>(0);
        builder.CloseComponent();

        builder.OpenComponent<MudDialogProvider>(1);
        builder.CloseComponent();

        builder.OpenComponent<MudSnackbarProvider>(2);
        builder.CloseComponent();

        builder.AddContent(3, ChildContent);
    }
}
