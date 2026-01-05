using FluentAssertions;
using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.EFCore.Seeding;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

public class LocalizationSeederOverwriteTests
{
    private IDbContextFactory<GeauxLocalizationDbContext> CreateFactory()
    {
        DbContextOptions<GeauxLocalizationDbContext> options = new DbContextOptionsBuilder<GeauxLocalizationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PooledDbContextFactory<GeauxLocalizationDbContext>(options);
    }

    [Fact]
    public async Task OverwriteExisting_UpdatesValue()
    {
        IDbContextFactory<GeauxLocalizationDbContext> factory = CreateFactory();
        Assembly[] assemblies = new[] { typeof(TestLocalizedModel).Assembly };
        Type[] modelTypes = new[] { typeof(TestLocalizedModel) };
        var cultures = new[] { "en-US" };

        var seeder = new KeySeeder(assemblies);

        // First run
        await seeder.SeedAsync(factory, modelTypes, cultures);

        // Modify value
        await using (GeauxLocalizationDbContext db = await factory.CreateDbContextAsync())
        {
            LocalizationValue value = await db.LocalizationValues.FirstAsync();
            value.Value = "OLD";
            await db.SaveChangesAsync();
        }

        // Second run with overwrite
        await seeder.SeedAsync(factory, modelTypes, cultures, overwriteExisting: true);

        await using GeauxLocalizationDbContext db2 = await factory.CreateDbContextAsync();
        LocalizationValue updated = await db2.LocalizationValues.FirstAsync();

        updated.Value.Should().Be("Test.Name");
    }

    private sealed class TestLocalizedModel
    {
        [Localized("Test.Name")]
        public string Name { get; set; } = "";
    }
}
