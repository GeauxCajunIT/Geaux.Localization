using FluentAssertions;
using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
        conn.Open();

        var options = new DbContextOptionsBuilder<Geaux.Localization.Contexts.GeauxLocalizationDbContext>()
            .UseSqlite(conn)
            .Options;

        await using GeauxLocalizationDbContext db = new Geaux.Localization.Contexts.GeauxLocalizationDbContext(options);
        await db.Database.EnsureCreatedAsync();

        LocalizationSeeder seeder = new LocalizationSeeder(db, new[] { Assembly.GetExecutingAssembly() });

        await seeder.SeedAsync("en-US", tenantId: "T1");

        var count1 = await db.LocalizationValues.CountAsync();
        count1.Should().BeGreaterThanOrEqualTo(3);

        await seeder.SeedAsync("en-US", tenantId: "T1");
        var count2 = await db.LocalizationValues.CountAsync();

        count2.Should().Be(count1);
    }
}
