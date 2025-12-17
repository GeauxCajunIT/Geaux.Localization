namespace Geaux.Localization.Config;

/// <summary>
/// Options controlling the behavior of the Geaux localization system.
/// </summary>
public sealed class LocalizationOptions
{
    /// <summary>
    /// Gets or sets the default culture to use when no explicit culture is resolved.
    /// </summary>
    public string DefaultCulture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the list of supported cultures for validation and UI culture filtering.
    /// </summary>
    public IList<string> SupportedCultures { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether to fall back to <see cref="DefaultCulture"/> when a translation is
    /// missing for the requested culture.
    /// </summary>
    public bool EnableCultureFallback { get; set; } = true;

    /// <summary>
    /// Gets or sets the connection string name in configuration used for the localization database.
    /// </summary>
    public string ConnectionStringName { get; set; } = "LocalizationDb";

    /// <summary>
    /// Gets or sets the database provider identifier used to configure the DbContext.
    /// Supported values: SqlServer, PostgreSql, MySql, Sqlite, LocalDb.
    /// </summary>
    public string Provider { get; set; } = "SqlServer";

    /// <summary>
    /// Gets or sets the optional migrations assembly name to use for EF Core migrations.
    /// Typically points to the Geaux.Migrations project.
    /// </summary>
    public string? MigrationsAssembly { get; set; }

    /// <summary>
    /// Gets or sets an optional prefix to use when generating translation keys.
    /// </summary>
    public string? KeyPrefix { get; set; }
}
