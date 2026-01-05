using Bunit;
using FluentAssertions;
using Geaux.Localization.Admin.Pages;
using Geaux.Localization.Admin.Tests;
using Xunit;

public class DashboardTests : AdminTestContext
{
    [Fact]
    public void DashboardLoads()
    {
        IRenderedComponent<Dashboard> cut = Render<Dashboard>();
        cut.Markup.Should().Contain("Translations");
    }
}
