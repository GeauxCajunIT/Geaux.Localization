
using FluentAssertions;
using Geaux.Localization.Config;
using Geaux.Localization.Extensions;
using Geaux.Localization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Geaux.Localization.Tests
{
    public class LocalizationSaveChangesInterceptorTests
    {
        [Fact]
        public void Interceptor_IsRegisteredAndCanBeResolved()
        {
            KeyValuePair<string, string?>[] inMemory = new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:LocalizationDb", "DataSource=:memory:"),
                new KeyValuePair<string, string?>("Localization:ConnectionStringName", "LocalizationDb"),
                new KeyValuePair<string, string?>("Localization:Provider", "sqlite")
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalization(config.GetSection("Localization"));

            ServiceProvider sp = services.BuildServiceProvider();

            LocalizationSaveChangesInterceptor? interceptor = sp.GetService<LocalizationSaveChangesInterceptor>();
            interceptor.Should().NotBeNull();
        }

        [Fact]
        public void Options_AreConfiguredAndAvailableViaIOptions()
        {
            KeyValuePair<string, string?>[] inMemory = new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:LocalizationDb", "DataSource=:memory:"),
                new KeyValuePair<string, string?>("Localization:ConnectionStringName", "LocalizationDb"),
                new KeyValuePair<string, string?>("Localization:Provider", "sqlite"),
                new KeyValuePair<string, string?>("Localization:MigrationsAssembly", "Geaux.Migrations")
            };

            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalization(config.GetSection("Localization"));

            ServiceProvider sp = services.BuildServiceProvider();
            LocalizationOptions opts = sp.GetRequiredService<IOptions<LocalizationOptions>>().Value;

            opts.ConnectionStringName.Should().Be("LocalizationDb");
            opts.Provider.Should().Be("sqlite");
            opts.MigrationsAssembly.Should().Be("Geaux.Migrations");
        }
    }
}



