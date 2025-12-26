using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geaux.Localization.Extensions;

/// <summary>
/// Runtime helpers for initializing the Geaux.Localization database.
/// </summary>
public static class LocalizationDatabaseInitializer
{
    /// <summary>
    /// Initializes the localization database.
    /// <para>
    /// - SQLite → EnsureCreated
    /// - SQL Server (and others) → Migrate
    /// </para>
    /// </summary>
    /// <param name="services">The application service provider used to resolve dependencies.</param>
    public static async Task InitializeGeauxLocalizationDatabaseAsync(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        IDbContextFactory<GeauxLocalizationDbContext> factory = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();

        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync();

        var provider = db.Database.ProviderName ?? string.Empty;

        if (provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            await db.Database.MigrateAsync();
        }
    }
}
