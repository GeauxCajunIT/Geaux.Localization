using Microsoft.Extensions.Configuration;

internal static class TestConfig
{
    public static IConfiguration WithLocalizationConn(string conn = "Data Source=:memory:")
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:LocalizationConnection"] = conn
            })
            .Build();
}
