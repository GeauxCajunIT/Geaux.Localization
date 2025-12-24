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
    /// Gets or sets an optional tenant identifier used to scope translations.
    /// </summary>
    /// <remarks>
    /// When set, database queries will include this value so that each tenant can have its own
    /// translation set. When null or empty, translations are treated as global.
    /// </remarks>
    public string? TenantId { get; set; }

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
    /// Full connection string to the localization database. If set, this takes precedence.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Name of the connection string in IConfiguration's ConnectionStrings section.
    /// Default: "LocalizationDb".
    /// </summary>
    public string ConnectionStringName { get; set; } = "LocalizationDb";

    /// <summary>
    /// Database provider hint: "SqlServer", "Npgsql", "Sqlite", "MySql", "LocalDb".
    /// If empty, defaults to SqlServer.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Optional migrations assembly name to use when configuring EF.
    /// If null, the assembly of the DbContext will be used.
    /// </summary>
    public string? MigrationsAssembly { get; set; }

    /// <summary>
    /// Gets or sets an optional prefix to use when generating translation keys.
    /// </summary>
    public string? KeyPrefix { get; set; }
}
