// // <copyright file="" company="GeauxCajunIT">
// // Copyright (c) GeauxCajunIT. All rights reserved.
// // </copyright>

namespace Geaux.Localization.Config;

/// <summary>
/// Options controlling the behavior of the Geaux localization system.
/// </summary>
public sealed class LocalizationOptions
{
    /// <summary>
    /// The default culture to use when no explicit culture is resolved.
    /// </summary>
    public string DefaultCulture { get; set; } = "en-US";

    /// <summary>
    /// Optional list of supported cultures for validation and UI culture filtering.
    /// </summary>
    public IList<string> SupportedCultures { get; set; } = new List<string>();

    /// <summary>
    /// If true, fall back to <see cref="DefaultCulture"/> when a translation is missing for the requested culture.
    /// </summary>
    public bool EnableCultureFallback { get; set; } = true;

    /// <summary>
    /// The connection string name in configuration used for the localization database.
    /// </summary>
    public string ConnectionStringName { get; set; } = "LocalizationDb";

    /// <summary>
    /// Database provider identifier used to configure the DbContext.
    /// Supported values: SqlServer, PostgreSql, MySql, Sqlite, LocalDb.
    /// </summary>
    public string Provider { get; set; } = "SqlServer";

    /// <summary>
    /// Optional migrations assembly name to use for EF Core migrations.
    /// Typically points to the Geaux.Migrations project.
    /// </summary>
    public string? MigrationsAssembly { get; set; }

    /// <summary>
    /// Optional prefix to use when generating translation keys.
    /// </summary>
    public string? KeyPrefix { get; set; }
}

