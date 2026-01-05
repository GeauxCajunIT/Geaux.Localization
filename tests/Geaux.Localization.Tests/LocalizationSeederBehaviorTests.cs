using FluentAssertions;
using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.EFCore.Seeding;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

public class LocalizationSeederBehaviorTests
{
    private IDbContextFactory<GeauxLocalizationDbContext> CreateFactory()
    {
        DbContextOptions<GeauxLocalizationDbContext> options = new DbContextOptionsBuilder<GeauxLocalizationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PooledDbContextFactory<GeauxLocalizationDbContext>(options);
    }

    [Fact]
    public async Task Seeder_CreatesMissingKeys_And_DefaultValues()
    {
        // Arrange
        IDbContextFactory<GeauxLocalizationDbContext> factory = CreateFactory();

        Assembly[] assemblies = new[] { typeof(TestLocalizedModel).Assembly };
        Type[] modelTypes = new[] { typeof(TestLocalizedModel) };
        var cultures = new[] { "en-US", "fr-FR" };

        var seeder = new KeySeeder(assemblies);

        // Act
        await seeder.SeedAsync(factory, modelTypes, cultures);

        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync();

        // Assert: Keys created
        List<LocalizationKey> keys = await db.LocalizationKeys.ToListAsync();
        keys.Should().NotBeEmpty();
        keys.Should().Contain(k => k.Key == "Test.Name");

        // Assert: Values created for each culture
        List<LocalizationValue> values = await db.LocalizationValues.ToListAsync();
        values.Should().HaveCount(cultures.Length);

        values.Should().Contain(v => v.Culture == "en-US");
        values.Should().Contain(v => v.Culture == "fr-FR");
    }

    [Fact]
    public async Task Seeder_DoesNotDuplicateKeys_OnSecondRun()
    {
        // Arrange
        IDbContextFactory<GeauxLocalizationDbContext> factory = CreateFactory();

        Assembly[] assemblies = new[] { typeof(TestLocalizedModel).Assembly };
        Type[] modelTypes = new[] { typeof(TestLocalizedModel) };
        var cultures = new[] { "en-US" };

        var seeder = new KeySeeder(assemblies);

        // Act
        await seeder.SeedAsync(factory, modelTypes, cultures);
        await seeder.SeedAsync(factory, modelTypes, cultures);

        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync();

        // Assert
        List<LocalizationKey> keys = await db.LocalizationKeys.ToListAsync();
        keys.Should().HaveCount(1);

        List<LocalizationValue> values = await db.LocalizationValues.ToListAsync();
        values.Should().HaveCount(1);
    }

    // Test model with LocalizedAttribute
    private sealed class TestLocalizedModel
    {
        [Localized("Test.Name")]
        public string Name { get; set; } = "";
    }
}
