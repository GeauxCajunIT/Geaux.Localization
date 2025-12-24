
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Geaux.Localization.Tests
{
    public static class TestHelpers
    {
        public static IConfiguration BuildConfiguration(params KeyValuePair<string, string?>[] values)
        {
            return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        }

        public static ServiceProvider BuildServiceProviderWithConfig(params KeyValuePair<string, string?>[] values)
        {
            IConfiguration config = BuildConfiguration(values);
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            return services.BuildServiceProvider();
        }
    }
}



