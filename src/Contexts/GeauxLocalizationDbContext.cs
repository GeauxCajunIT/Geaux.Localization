using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Contexts;

/// <summary>
/// Entity Framework Core database context for managing localization data.
/// </summary>
/// <remarks>
/// The model enforces uniqueness on <c>(TenantId, Culture, Key)</c>.
/// </remarks>
public sealed class GeauxLocalizationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeauxLocalizationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public GeauxLocalizationDbContext(DbContextOptions<GeauxLocalizationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LocalizationKey> LocalizationKeys => Set<LocalizationKey>();
    public DbSet<LocalizationValue> LocalizationValues => Set<LocalizationValue>();

    /// <summary>
    /// Gets the collection of translations.
    /// </summary>
    /// [Obsolete("Use LocalizationKeys / LocalizationValues")]
    public DbSet<Translation> Translations => Set<Translation>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocalizationKey>(e =>
        {
            e.HasIndex(x => x.Key).IsUnique();
        });

        modelBuilder.Entity<LocalizationValue>(e =>
        {
            e.HasOne(v => v.LocalizationKey)
             .WithMany(k => k.Values)
             .HasForeignKey(v => v.LocalizationKeyId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(v => new { v.LocalizationKeyId, v.Culture })
             .IsUnique()
             .HasFilter("[TenantId] IS NULL");

            e.HasIndex(v => new { v.TenantId, v.LocalizationKeyId, v.Culture })
             .IsUnique()
             .HasFilter("[TenantId] IS NOT NULL");
        });
    }
}
