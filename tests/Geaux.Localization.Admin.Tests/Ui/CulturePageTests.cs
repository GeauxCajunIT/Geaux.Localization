using Bunit;
using FluentAssertions;
using Geaux.Localization.Admin.Pages.Cultures;
using Geaux.Localization.Admin.Tests;
using Geaux.Localization.Admin.Tests.Infrastructure;
using Xunit;

public class CulturePageTests : AdminTestContext
{
    [Fact]
    public void CulturePageRenders()
    {
        IRenderedComponent<MudTestHost> cut = Render<MudTestHost>(p => p.AddChildContent<CultureList>());

        cut.Markup.Should().Contain("Search cultures");
    }
}
