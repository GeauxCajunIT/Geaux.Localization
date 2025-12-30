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
        public void Interceptor_IsRegisteredAndResolvable()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalization(config, opts =>
            {
                opts.Provider = "Sqlite";
                opts.ConnectionString = "Data Source=:memory:"; // ✅ avoid config lookup
                opts.UseDbContextFactory = true;
            });

            using ServiceProvider sp = services.BuildServiceProvider();

            // NOTE: resolve inside scope since interceptor is usually scoped
            using IServiceScope scope = sp.CreateScope();
            LocalizationSaveChangesInterceptor interceptor = scope.ServiceProvider.GetRequiredService<LocalizationSaveChangesInterceptor>();

            Assert.NotNull(interceptor);
        }

        [Fact]
        public void Options_AreConfiguredAndAvailableViaIOptions()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            services.AddGeauxLocalization(config, opts =>
            {
                opts.Provider = "Sqlite";
                opts.ConnectionString = "Data Source=:memory:"; // ✅ required
                opts.ConnectionStringName = "LocalizationConnection";
                opts.MigrationsAssembly = "Geaux.Localization";
                opts.AutoMigrate = true;
                opts.UseDbContextFactory = true;
            });

            using ServiceProvider sp = services.BuildServiceProvider();
            GeauxLocalizationOptions resolved = sp.GetRequiredService<IOptions<GeauxLocalizationOptions>>().Value;

            Assert.Equal("LocalizationConnection", resolved.ConnectionStringName);
            Assert.Equal("Geaux.Localization", resolved.MigrationsAssembly);
            Assert.True(resolved.AutoMigrate);
            Assert.True(resolved.UseDbContextFactory);
            Assert.Equal("Sqlite", resolved.Provider);
        }

    }
}



