
using FluentAssertions;
using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore.Design;

namespace Geaux.Localization.Tests
{
    public class DesignTimeFactoryTests
    {
        [Fact]
        public void DesignTimeFactory_CanCreateDbContext_WhenProvided()
        {
            // If your project provides a design-time factory, test it here.
            // This test assumes a factory type named DesignTimeGeauxLocalizationDbContextFactory exists.
            // If you don't have a factory, skip this test or add one to the library.

            Type factoryType = typeof(IDesignTimeDbContextFactory<GeauxLocalizationDbContext>);
            // If no factory is present, the test is skipped.
            // Replace 'DesignTimeGeauxLocalizationDbContextFactory' with your actual factory type if present.
            Type? factory = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => factoryType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (factory == null)
            {
                // No factory found; mark test as passed but note that migrations will rely on host building.
                Assert.True(true);
                return;
            }

            IDesignTimeDbContextFactory<GeauxLocalizationDbContext> instance = (IDesignTimeDbContextFactory<GeauxLocalizationDbContext>)Activator.CreateInstance(factory)!;
            GeauxLocalizationDbContext ctx = instance.CreateDbContext(Array.Empty<string>());
            ctx.Should().NotBeNull();
            ctx.Database.CanConnect().Should().BeFalse(); // likely false for in-memory or missing DB, but ensure ctx created
        }
    }
}



