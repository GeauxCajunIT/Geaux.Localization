
using FluentAssertions;
using Geaux.Localization.Config;
using Microsoft.Extensions.Configuration;

namespace Geaux.Localization.Tests
{
    public class LocalizationOptionsTests
    {
        [Fact]
        public void Bind_FromConfigurationSection_PopulatesOptions()
        {
            KeyValuePair<string, string?>[] inMemory = new[]
            {
                new KeyValuePair<string, string?>("Localization:ConnectionStringName", "LocalizationDb"),
                new KeyValuePair<string, string?>("Localization:Provider", "Sqlite"),
                new KeyValuePair<string, string?>("Localization:MigrationsAssembly", "Geaux.Migrations")
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            IConfigurationSection section = config.GetSection("Localization");

            LocalizationOptions options = new LocalizationOptions();
            section.Bind(options);

            options.ConnectionStringName.Should().Be("LocalizationDb");
            options.Provider.Should().Be("Sqlite");
            options.MigrationsAssembly.Should().Be("Geaux.Migrations");
        }

        [Fact]
        public void Default_ConnectionStringName_IsLocalizationDb()
        {
            LocalizationOptions opts = new LocalizationOptions();
            opts.ConnectionStringName.Should().Be("LocalizationDb");
        }
    }
}



