using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class KeyApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public KeyApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanListKeys()
    {
        HttpResponseMessage response = await _client.GetAsync("/admin/localization/api/keys");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<object>? keys = await response.Content.ReadFromJsonAsync<List<object>>();
        keys.Should().NotBeNull();
    }
}
