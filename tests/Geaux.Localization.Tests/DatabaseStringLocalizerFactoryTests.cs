
using FluentAssertions;
using Geaux.Localization.Extensions;
using Geaux.Localization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.Tests
{
    public class DatabaseStringLocalizerFactoryTests
    {
        [Fact]
        public void AddGeauxLocalization_RegistersDatabaseStringLocalizerFactory_AsIStringLocalizerFactory()
        {
            KeyValuePair<string, string?>[] inMemory = new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:LocalizationDb", "DataSource=:memory:"),
                new KeyValuePair<string, string?>("Localization:ConnectionStringName", "LocalizationDb"),
                new KeyValuePair<string, string?>("Localization:Provider", "sqlite")
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalization(config.GetSection("Localization"));

            ServiceProvider sp = services.BuildServiceProvider();

            IStringLocalizerFactory? factory = sp.GetService<IStringLocalizerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<DatabaseStringLocalizerFactory>();
        }

        [Fact]
        public void DatabaseStringLocalizerFactory_CanCreateLocalizer_ForType()
        {
            KeyValuePair<string, string?>[] inMemory = new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:LocalizationDb", "DataSource=:memory:"),
                new KeyValuePair<string, string?>("Localization:ConnectionStringName", "LocalizationDb"),
                new KeyValuePair<string, string?>("Localization:Provider", "sqlite")
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            services.AddGeauxLocalization(config.GetSection("Localization"));

            ServiceProvider sp = services.BuildServiceProvider();
            IStringLocalizerFactory factory = sp.GetRequiredService<IStringLocalizerFactory>();

            // Should not throw when creating a localizer for a type
            IStringLocalizer localizer = factory.Create(typeof(DatabaseStringLocalizerFactory));
            localizer.Should().NotBeNull();
        }
    }
}



