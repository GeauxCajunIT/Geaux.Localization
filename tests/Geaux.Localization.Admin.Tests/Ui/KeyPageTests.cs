using Bunit;
using FluentAssertions;
using Geaux.Localization.Admin.Pages.Keys;
using Geaux.Localization.Admin.Tests.Infrastructure;
using Xunit;

public class KeyPageTests : BunitTestContextBase
{
    [Fact]
    public void KeyPageRenders()
    {
        IRenderedComponent<MudTestHost> cut = Render<MudTestHost>(p => p.AddChildContent<KeyList>());

        cut.Markup.Should().Contain("Search keys");
    }
}
