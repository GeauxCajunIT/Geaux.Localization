using FluentAssertions;
using Geaux.Localization.Config;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Localization.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Geaux.Localization.Tests;

public class DatabaseStringLocalizerBehaviorTests
{
    private static PooledDbContextFactory<GeauxLocalizationDbContext> CreateFactory(SqliteConnection conn)
    {
        DbContextOptions<GeauxLocalizationDbContext> options = new DbContextOptionsBuilder<GeauxLocalizationDbContext>()
            .UseSqlite(conn)
            .Options;

        using (GeauxLocalizationDbContext db = new GeauxLocalizationDbContext(options))
        {
            db.Database.EnsureCreated();
        }

        return new PooledDbContextFactory<GeauxLocalizationDbContext>(options);
    }

    private static int SeedKey(GeauxLocalizationDbContext db, string key)
    {
        LocalizationKey k = new LocalizationKey { Key = key };
        db.LocalizationKeys.Add(k);
        db.SaveChanges();
        return k.Id;
    }

    private static DatabaseStringLocalizer CreateLocalizer(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        string culture,
        string? tenantId = null)
    {
        var options = Options.Create(new LocalizationOptions
        {
            DefaultCulture = "en-US",
            EnableCultureFallback = true,
            TenantId = tenantId
        });

        return new DatabaseStringLocalizer(
            factory,
            options,
            resourceName: "Tests",
            tenantId: tenantId,
            culture: new CultureInfo(culture));
    }

    [Fact]
    public void Indexer_prefers_tenant_value_over_global()
    {
        using SqliteConnection conn = new SqliteConnection("Filename=:memory:");
        conn.Open();

        var factory = CreateFactory(conn);

        using (var db = factory.CreateDbContext())
        {
            var keyId = SeedKey(db, "Hello");

            db.LocalizationValues.AddRange(
                new LocalizationValue { LocalizationKeyId = keyId, TenantId = null, Culture = "en-US", Value = "Hello (Global)" },
                new LocalizationValue { LocalizationKeyId = keyId, TenantId = "T1", Culture = "en-US", Value = "Hello (Tenant T1)" }
            );

            db.SaveChanges();
        }

        var localizer = CreateLocalizer(factory, "en-US", "T1");

        localizer["Hello"].ResourceNotFound.Should().BeFalse();
        localizer["Hello"].Value.Should().Be("Hello (Tenant T1)");
    }

    [Fact]
    public void Indexer_falls_back_to_global_when_tenant_value_missing()
    {
        using SqliteConnection conn = new SqliteConnection("Filename=:memory:");
        conn.Open();

        var factory = CreateFactory(conn);

        using (var db = factory.CreateDbContext())
        {
            var keyId = SeedKey(db, "Hello");

            db.LocalizationValues.Add(
                new LocalizationValue { LocalizationKeyId = keyId, TenantId = null, Culture = "en-US", Value = "Hello (Global)" });

            db.SaveChanges();
        }

        var localizer = CreateLocalizer(factory, "en-US", "T1");

        localizer["Hello"].Value.Should().Be("Hello (Global)");
    }

    [Fact]
    public void WithCulture_overrides_culture_for_lookup()
    {
        using SqliteConnection conn = new SqliteConnection("Filename=:memory:");
        conn.Open();

        var factory = CreateFactory(conn);

        using (var db = factory.CreateDbContext())
        {
            var keyId = SeedKey(db, "Greet");

            db.LocalizationValues.AddRange(
                new LocalizationValue { LocalizationKeyId = keyId, TenantId = null, Culture = "en-US", Value = "Hello" },
                new LocalizationValue { LocalizationKeyId = keyId, TenantId = null, Culture = "fr-FR", Value = "Bonjour" }
            );

            db.SaveChanges();
        }

        var localizer = CreateLocalizer(factory, "en-US")
            .WithCulture(new CultureInfo("fr-FR"));

        localizer["Greet"].Value.Should().Be("Bonjour");
    }

    [Fact]
    public void GetAllStrings_when_includeParentCultures_true_includes_parent_culture_values()
    {
        using SqliteConnection conn = new SqliteConnection("Filename=:memory:");
        conn.Open();

        var factory = CreateFactory(conn);

        using (var db = factory.CreateDbContext())
        {
            var kid1 = SeedKey(db, "OnlyParent");
            var kid2 = SeedKey(db, "OnlyChild");

            db.LocalizationValues.AddRange(
                new LocalizationValue { LocalizationKeyId = kid1, TenantId = null, Culture = "fr", Value = "ParentValue" },
                new LocalizationValue { LocalizationKeyId = kid2, TenantId = null, Culture = "fr-CA", Value = "ChildValue" }
            );

            db.SaveChanges();
        }

        var localizer = CreateLocalizer(factory, "fr-CA");

        Dictionary<string, string> all = localizer.GetAllStrings(includeParentCultures: true)
            .ToDictionary(x => x.Name, x => x.Value);

        all.Should().ContainKey("OnlyChild").WhoseValue.Should().Be("ChildValue");
        all.Should().ContainKey("OnlyParent").WhoseValue.Should().Be("ParentValue");
    }
}
