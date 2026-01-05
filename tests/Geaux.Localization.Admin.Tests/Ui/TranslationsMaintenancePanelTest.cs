using Bunit;
using FluentAssertions;
using Geaux.Localization.Admin.Pages;
using Geaux.Localization.Admin.Tests.Infrastructure;
using Xunit;

public class TranslationsMaintenancePanelTests : BunitTestContextBase
{
    [Fact]
    public void PanelRenders()
    {
        IRenderedComponent<MudTestHost> cut = Render<MudTestHost>(p => p.AddChildContent<TranslationsMaintenancePanel>());

        cut.Markup.Should().Contain("Upload Language Pack");
    }
}

