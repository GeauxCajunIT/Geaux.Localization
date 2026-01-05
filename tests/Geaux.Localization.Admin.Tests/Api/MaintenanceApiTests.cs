using FluentAssertions;
using System.Net;
using Xunit;

public class MaintenanceApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MaintenanceApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanTriggerMaintenance()
    {
        HttpResponseMessage response = await _client.PostAsync("/admin/localization/api/maintenance/run", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
