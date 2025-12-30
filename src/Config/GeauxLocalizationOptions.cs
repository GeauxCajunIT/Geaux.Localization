namespace Geaux.Localization.Config;

public sealed class GeauxLocalizationOptions
{
    /// <summary>
    /// Optional explicit connection string. If not provided, <see cref="ConnectionStringName"/> is used.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Name of the connection string in configuration under ConnectionStrings:{name}.
    /// Default: LocalizationConnection
    /// </summary>
    public string ConnectionStringName { get; set; } = "LocalizationConnection";

    /// <summary>
    /// Optional migrations assembly name for GeauxLocalizationDbContext.
    /// If null/empty, defaults to the DbContext assembly.
    /// </summary>
    public string? MigrationsAssembly { get; set; } = "Geaux.Localization";

    /// <summary>
    /// If true, runs DbContext.Database.Migrate() at startup (recommended for samples/dev).
    /// Default: false
    /// </summary>
    public bool AutoMigrate { get; set; } = false;

    /// <summary>
    /// If true, registers IDbContextFactory&lt;GeauxLocalizationDbContext&gt; instead of DbContext directly.
    /// Default: true
    /// </summary>
    public bool UseDbContextFactory { get; set; } = true;

    /// <summary>
    /// If true, localization lookups can fallback to a default culture when a specific culture value is missing.
    /// Default: true
    /// </summary>
    public bool EnableCultureFallback { get; set; } = true;

    /// <summary>
    /// Default culture used for fallback behavior.
    /// Default: en
    /// </summary>
    public string DefaultCulture { get; set; } = "en";

    /// <summary>
    /// Optional tenant id override. If null, implementation may resolve from tenant context accessor.
    /// Kept for backward compatibility.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Provider name: SqlServer, Sqlite, Npgsql, MySql.
    /// </summary>
    public string? Provider { get; set; } = "SqlServer";

    public bool AutoSeedLocalizedAttributes { get; set; } = true;

    public bool SeedOverwriteExisting { get; set; } = true;

    public IReadOnlyCollection<string> SupportedCultures { get; set; } = Array.Empty<string>();

    public IReadOnlyCollection<Type> ModelTypes { get; set; } = Array.Empty<Type>();

    /// <summary>Enable SQL Server retry-on-failure (recommended for cloud). Only applies to SqlServer.</summary>
    public bool EnableRetryOnFailure { get; set; } = true;

    /// <summary>Throw if EF detects PendingModelChangesWarning (useful to fail fast in dev/CI).</summary>
    public bool ThrowOnPendingModelChanges { get; set; } = false;

}
