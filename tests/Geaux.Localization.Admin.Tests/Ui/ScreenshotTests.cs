using Microsoft.Playwright;
using Xunit;

public class ScreenshotTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public ScreenshotTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DashboardScreenshot()
    {
        using HttpClient client = _factory.CreateClient();
        string baseUrl = client.BaseAddress!.ToString().TrimEnd('/');

        using IPlaywright playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        IPage page = await browser.NewPageAsync();
        await page.GotoAsync($"{baseUrl}/admin/localization");

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "Dashboard.png",
            FullPage = true
        });
    }
}
