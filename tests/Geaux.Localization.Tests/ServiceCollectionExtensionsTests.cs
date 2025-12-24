
using FluentAssertions;
using Geaux.Localization.Contexts;
using Geaux.Localization.Extensions;
using Geaux.Localization.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddGeauxLocalization_WithExplicitConnectionString_RegistersServicesAndDbContext()
        {
            ServiceCollection services = new ServiceCollection();

            // Use in-memory SQLite connection string for testing
            string conn = "DataSource=:memory:";
            services.AddGeauxLocalization(opts =>
            {
                opts.ConnectionString = conn;
                opts.Provider = "sqlite";
                opts.MigrationsAssembly = typeof(GeauxLocalizationDbContext).Assembly.GetName().Name;
            });

            ServiceProvider sp = services.BuildServiceProvider();


            // Ensure IStringLocalizerFactory registered
            sp.GetService<IStringLocalizerFactory>().Should().NotBeNull();

            // Ensure interceptor registered
            sp.GetService<LocalizationSaveChangesInterceptor>().Should().NotBeNull();

            // Ensure DbContext resolves and can open connection
            using IServiceScope scope = sp.CreateScope();
            GeauxLocalizationDbContext ctx = scope.ServiceProvider.GetRequiredService<GeauxLocalizationDbContext>();
            ctx.Database.GetDbConnection().ConnectionString.Should().Be(conn);

            // Try to open the connection (SQLite in-memory requires opening)
            ctx.Database.OpenConnection();
            ctx.Database.CloseConnection();
        }

        [Fact]
        public void AddGeauxLocalization_WithConfigurationSection_ResolvesConnectionStringByName()
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

            // Use the IConfiguration section overload
            services.AddGeauxLocalization(config.GetSection("Localization"));

            ServiceProvider sp = services.BuildServiceProvider();

            // DbContext resolves
            using IServiceScope scope = sp.CreateScope();
            GeauxLocalizationDbContext ctx = scope.ServiceProvider.GetRequiredService<GeauxLocalizationDbContext>();
            ctx.Should().NotBeNull();
        }

        [Fact]
        public void AddGeauxLocalization_ThrowsWhenConnectionStringMissing()
        {
            ServiceCollection services = new ServiceCollection();

            Config.LocalizationOptions options = new Config.LocalizationOptions
            {
                ConnectionStringName = "NonExistentName",
                Provider = "sqlite"
            };

            Action act = () => services.AddGeauxLocalization(options, configuration: null);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*NonExistentName*");
        }
    }
}



