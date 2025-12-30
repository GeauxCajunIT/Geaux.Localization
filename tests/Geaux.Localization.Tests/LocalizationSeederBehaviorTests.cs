using FluentAssertions;
using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace Geaux.Localization.Tests;

public class LocalizationSeederBehaviorTests
{
    private sealed class SeedModel
    {
        [Localized("Seed.Key", DisplayNameKey = "Seed.DisplayName", ErrorMessageKey = "Seed.Error")]
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task SeedAsync_creates_missing_keys_and_is_idempotent()
    {
        using SqliteConnection conn = new SqliteConnection("Filename=:memory:");
        await conn.OpenAsync();

        DbContextOptions<GeauxLocalizationDbContext> options = new DbContextOptionsBuilder<GeauxLocalizationDbContext>()
            .UseSqlite(conn)
            .Options;

        // Create schema once
        await using (var db = new GeauxLocalizationDbContext(options))
        {
            await db.Database.EnsureCreatedAsync();
        }

        // Factory for the seeder (prevents disposed context issues)
        var factory = new PooledDbContextFactory<GeauxLocalizationDbContext>(options);

        // New seeder ctor only takes assemblies
        LocalizationSeeder seeder = new LocalizationSeeder(new[] { Assembly.GetExecutingAssembly() });

        // Seed first time (scan assemblies)
        await seeder.SeedAsync(
            factory: factory,
            modelTypes: Array.Empty<Type>(),              // empty => scan assemblies passed to ctor
            supportedCultures: new[] { "en-US" },
            tenantId: "T1",
            overwriteExisting: false);

        // Verify values created
        await using (var db = new GeauxLocalizationDbContext(options))
        {
            var count1 = await db.LocalizationValues.CountAsync();
            count1.Should().BeGreaterThanOrEqualTo(3);

            // Seed again => idempotent
            await seeder.SeedAsync(
                factory: factory,
                modelTypes: Array.Empty<Type>(),
                supportedCultures: new[] { "en-US" },
                tenantId: "T1",
                overwriteExisting: false);

            var count2 = await db.LocalizationValues.CountAsync();
            count2.Should().Be(count1);
        }
    }
}
