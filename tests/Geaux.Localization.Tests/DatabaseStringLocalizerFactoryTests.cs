
using FluentAssertions;
using Geaux.Localization.Extensions;
using Geaux.Localization.Services.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.Tests
{
    public class DatabaseStringLocalizerFactoryTests
    {
        [Fact]
        public void AddGeauxLocalization_Registers_IStringLocalizerFactory()
        {
            Dictionary<string, string?> inMemory = new Dictionary<string, string?>
            {
                ["ConnectionStrings:LocalizationConnection"] = "DataSource=:memory:"
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            ServiceCollection services = new ServiceCollection();

            services.AddGeauxLocalizationCore(config);

            ServiceProvider sp = services.BuildServiceProvider();

            IStringLocalizerFactory factory = sp.GetRequiredService<IStringLocalizerFactory>();
            factory.Should().BeOfType<LocalizationStringLocalizerFactory>();
        }


        [Fact]
        public void DatabaseStringLocalizerFactory_CanCreateLocalizer_ForType()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalizationCore(config, opts =>
            {
                opts.Provider = "Sqlite";
                opts.ConnectionString = "Data Source=:memory:"; // ✅ required
                opts.UseDbContextFactory = true;
            });

            using ServiceProvider sp = services.BuildServiceProvider();

            IStringLocalizerFactory factory = sp.GetRequiredService<IStringLocalizerFactory>();
            IStringLocalizer localizer = factory.Create(typeof(DatabaseStringLocalizerFactoryTests));

            Assert.NotNull(localizer);
        }
    }
}
