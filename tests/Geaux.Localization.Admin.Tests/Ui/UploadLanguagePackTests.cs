using Bunit;
using FluentAssertions;
using Geaux.Localization.Admin.Pages;
using Geaux.Localization.Admin.Tests.Infrastructure;
using Xunit;

public class UploadLanguagePackTests : BunitTestContextBase
{
    [Fact]
    public void UploadPageRenders()
    {
        IRenderedComponent<MudTestHost> cut = Render<MudTestHost>(p => p.AddChildContent<UploadLanguagePack>());

        cut.Markup.Should().Contain("Upload Language Pack");
    }
}
