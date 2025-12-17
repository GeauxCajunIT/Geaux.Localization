using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Data;

/// <summary>
/// Represents the Entity Framework Core database context for managing localization data, including translations,
/// within the application.
/// </summary>
/// <remarks>
/// This context provides access to translation entities and configures unique constraints to ensure that each
/// translation key is unique per tenant and culture. It is intended to be used with dependency injection and
/// configured with the appropriate database provider.
/// </remarks>
public class GeauxLocalizationContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeauxLocalizationContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext. Must not be null.</param>
    public GeauxLocalizationContext(DbContextOptions<GeauxLocalizationContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the collection of translations in the database context.
    /// </summary>
    /// <remarks>
    /// Use this property to query, add, update, or remove translation entities from the underlying data store.
    /// Changes made to this collection are tracked by the context and persisted to the database when
    /// <see cref="DbContext.SaveChanges()"/> is called.
    /// </remarks>
    public DbSet<Translation> Translations => Set<Translation>();

    /// <summary>
    /// Configures the entity model for the context.
    /// </summary>
    /// <remarks>
    /// This override configures the <see cref="Translation"/> entity to enforce uniqueness on the combination of
    /// tenant identifier, culture, and translation key. The configuration is applied once when the model is created.
    /// </remarks>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Translation>()
            .HasIndex(t => new { t.TenantId, t.Culture, t.Key })
            .IsUnique();
    }
}
