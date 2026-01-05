using FluentAssertions;
using Geaux.Localization.Contexts;
using Geaux.Localization.EFCore.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

public class KeySeederSeedAsyncSignatureTests
{
    private IDbContextFactory<GeauxLocalizationDbContext> CreateFactory()
    {
        DbContextOptions<GeauxLocalizationDbContext> options = new DbContextOptionsBuilder<GeauxLocalizationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PooledDbContextFactory<GeauxLocalizationDbContext>(options);
    }

    [Fact]
    public async Task SeedAsync_Method_IsCallable()
    {
        IDbContextFactory<GeauxLocalizationDbContext> factory = CreateFactory();
        Assembly[] assemblies = new[] { typeof(KeySeederSeedAsyncSignatureTests).Assembly };
        Type[] modelTypes = Array.Empty<Type>();
        var cultures = new[] { "en-US" };

        var seeder = new KeySeeder(assemblies);

        Func<Task> act = () => seeder.SeedAsync(factory, modelTypes, cultures);

        await act.Should().NotThrowAsync();
    }
}
