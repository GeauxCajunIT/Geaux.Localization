using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Force the content root to the project directory of the test project
        string projectDir = Directory.GetCurrentDirectory();
        builder.UseContentRoot(projectDir);

        builder.UseEnvironment("Development");
    }

}
