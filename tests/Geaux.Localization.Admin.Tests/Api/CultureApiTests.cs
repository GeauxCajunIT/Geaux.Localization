using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class CultureApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CultureApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanListCultures()
    {
        HttpResponseMessage response = await _client.GetAsync("/admin/localization/api/cultures");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<object>? cultures = await response.Content.ReadFromJsonAsync<List<object>>();
        cultures.Should().NotBeNull();
        cultures!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CanCreateCulture()
    {
        var payload = new { Name = "Test Culture", CultureCode = "xx-YY" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/admin/localization/api/cultures", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
