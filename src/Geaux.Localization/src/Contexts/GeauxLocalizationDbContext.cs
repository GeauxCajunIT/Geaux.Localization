using Geaux.Shared.Activity;

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
    public DbSet<LocalizationCulture> LocalizationCultures => Set<LocalizationCulture>();

    public DbSet<ActivityLog> LocalizationActivityLogs => Set<ActivityLog>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies all the EntityType Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocalizationCulturesEntityConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocalizationKeyEntityConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocalizationValueEntityConfiguration).Assembly);
    }
}
