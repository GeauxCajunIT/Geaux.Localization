
using FluentAssertions;
using Geaux.Localization.Config;
using Microsoft.Extensions.Configuration;

namespace Geaux.Localization.Tests
{
    public class GeauxLocalizationOptionsTests
    {
        [Fact]
        public void Bind_FromConfigurationSection_PopulatesOptions()
        {
            Dictionary<string, string?> inMemory = new Dictionary<string, string?>
            {
                ["Localization:ConnectionStringName"] = "LocalizationConnection",
                ["Localization:Provider"] = "Sqlite",
                ["Localization:MigrationsAssembly"] = "Geaux.Localization"
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            GeauxLocalizationOptions options = new GeauxLocalizationOptions();
            config.GetSection("Localization").Bind(options);

            options.ConnectionStringName.Should().Be("LocalizationConnection");
            options.Provider.Should().Be("Sqlite");
            options.MigrationsAssembly.Should().Be("Geaux.Localization");
        }

        [Fact]
        public void Default_ConnectionStringName_IsLocalizationConnection()
        {
            GeauxLocalizationOptions opts = new GeauxLocalizationOptions();
            opts.ConnectionStringName.Should().Be("LocalizationConnection");
        }
    }
}
