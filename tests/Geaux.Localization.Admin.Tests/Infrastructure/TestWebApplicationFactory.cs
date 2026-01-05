using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

namespace Geaux.Localization.Admin.Tests.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Force a stable content root and environment
        builder.UseSetting(WebHostDefaults.ContentRootKey, Directory.GetCurrentDirectory());
        builder.UseEnvironment("Development");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Double‑pin the content root so WebApplicationFactory can't override it
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        return base.CreateHost(builder);
    }
}
