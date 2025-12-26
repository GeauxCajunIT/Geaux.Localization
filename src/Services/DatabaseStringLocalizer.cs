using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using LocalizationOptions = Geaux.Localization.Config.LocalizationOptions;

namespace Geaux.Localization.Services;

/// <summary>
/// Provides database-backed string localization with tenant and culture precedence handling.
/// </summary>
public sealed class DatabaseStringLocalizer : IStringLocalizer
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _dbFactory;
    private readonly LocalizationOptions _options;
    private readonly string _resourceName;
    private readonly string? _tenantId;
    private readonly CultureInfo _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseStringLocalizer"/> class.
    /// </summary>
    /// <param name="dbFactory">Factory used to create the localization DbContext.</param>
    /// <param name="options">Localization options that determine culture fallback and tenant scoping.</param>
    /// <param name="resourceName">Resource name used for logging and context.</param>
    /// <param name="tenantId">Optional tenant identifier; null indicates global translations.</param>
    /// <param name="culture">Optional culture override; defaults to <see cref="CultureInfo.CurrentUICulture"/>.</param>
    public DatabaseStringLocalizer(
        IDbContextFactory<GeauxLocalizationDbContext> dbFactory,
        IOptions<LocalizationOptions> options,
        string resourceName,
        string? tenantId = null,
        CultureInfo? culture = null)
    {
        _dbFactory = dbFactory;
        _options = options.Value;
        _resourceName = resourceName;
        _tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;
        _culture = culture ?? CultureInfo.CurrentUICulture;
    }

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="name">The localization key to resolve.</param>
    public LocalizedString this[string name]
        => GetString(name);

    /// <summary>
    /// Gets the localized string for the specified key, formatting it with the provided arguments.
    /// </summary>
    /// <param name="name">The localization key to resolve.</param>
    /// <param name="arguments">Arguments used to format the localized value.</param>
    public LocalizedString this[string name, params object[] arguments]
        => GetString(name, arguments);

    /// <summary>
    /// Retrieves all localized strings, respecting the configured culture and tenant precedence.
    /// </summary>
    /// <param name="includeParentCultures">True to include parent cultures when evaluating fallbacks.</param>
    /// <returns>An enumeration of localized strings.</returns>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        using GeauxLocalizationDbContext db = _dbFactory.CreateDbContext();

        List<string> cultures = GetCultureChain(_culture, includeParentCultures);

        // Materialize from EF BEFORE disposing context
        var rows =
            (from v in db.LocalizationValues.AsNoTracking()
             where cultures.Contains(v.Culture)
             where _tenantId == null
                 ? v.TenantId == null
                 : v.TenantId == _tenantId || v.TenantId == null
             select new
             {
                 Key = v.LocalizationKey.Key,
                 v.Culture,
                 v.TenantId,
                 v.Value
             }).ToList();

        // Resolve precedence in-memory: tenant > global, specific culture > parent
        var resolved = rows
            .GroupBy(x => x.Key)
            .Select(g =>
                g.OrderBy(x => x.TenantId == null)          // tenant first
                 .ThenBy(x => cultures.IndexOf(x.Culture))  // culture priority
                 .First())
            .Select(x => new LocalizedString(x.Key, x.Value, false))
            .ToList();

        return resolved;
    }


    /// <summary>
    /// Resolves a localized string for the specified key.
    /// </summary>
    /// <param name="name">The localization key to resolve.</param>
    /// <param name="arguments">Optional formatting arguments.</param>
    /// <returns>A localized string representing the resolved value or the key when not found.</returns>
    private LocalizedString GetString(string name, params object[]? arguments)
    {
        using GeauxLocalizationDbContext db = _dbFactory.CreateDbContext();

        List<string> cultures = GetCultureChain(_culture, _options.EnableCultureFallback);

        foreach (string culture in cultures)
        {
            string? value =
                db.LocalizationValues.AsNoTracking()
                  .Where(v => v.LocalizationKey.Key == name &&
                              v.Culture == culture &&
                              (_tenantId == null
                                  ? v.TenantId == null
                                  : v.TenantId == _tenantId || v.TenantId == null))
                  .OrderBy(v => v.TenantId == null) // tenant first
                  .Select(v => v.Value)
                  .FirstOrDefault();

            if (value != null)
            {
                return arguments == null || arguments.Length == 0
                    ? new LocalizedString(name, value, false)
                    : new LocalizedString(name, string.Format(value, arguments), false);
            }
        }

        // Final fallback: DefaultCulture (if enabled and not already tried)
        if (_options.EnableCultureFallback &&
            !cultures.Contains(_options.DefaultCulture))
        {
            string? fallback =
                db.LocalizationValues.AsNoTracking()
                  .Where(v => v.LocalizationKey.Key == name &&
                              v.Culture == _options.DefaultCulture &&
                              (_tenantId == null
                                  ? v.TenantId == null
                                  : v.TenantId == _tenantId || v.TenantId == null))
                  .OrderBy(v => v.TenantId == null)
                  .Select(v => v.Value)
                  .FirstOrDefault();

            if (fallback != null)
            {
                return new LocalizedString(name, fallback, false);
            }
        }

        // Not found
        return new LocalizedString(name, name, true);
    }

    /// <summary>
    /// Builds the culture lookup chain for the requested culture.
    /// </summary>
    /// <param name="culture">The starting culture.</param>
    /// <param name="includeParents">True to include parent cultures.</param>
    /// <returns>The ordered set of culture names to evaluate.</returns>
    private static List<string> GetCultureChain(CultureInfo culture, bool includeParents)
    {
        List<string> result = new List<string> { culture.Name };

        if (!includeParents)
            return result;

        CultureInfo parent = culture.Parent;
        while (!string.IsNullOrEmpty(parent?.Name))
        {
            result.Add(parent.Name);
            parent = parent.Parent;
        }

        return result;
    }

    /// <summary>
    /// Creates a new <see cref="DatabaseStringLocalizer"/> scoped to the provided culture.
    /// </summary>
    /// <param name="culture">The culture to apply.</param>
    /// <returns>A localizer configured for the given culture.</returns>
    public DatabaseStringLocalizer WithCulture(CultureInfo culture)
        => new(_dbFactory,
               Microsoft.Extensions.Options.Options.Create(_options),
               _resourceName,
               _tenantId,
               culture);

    /// <summary>
    /// Creates a new <see cref="DatabaseStringLocalizer"/> scoped to the provided tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier; null for global scope.</param>
    /// <returns>A localizer configured for the given tenant.</returns>
    public DatabaseStringLocalizer WithTenant(string? tenantId)
        => new(_dbFactory,
               Microsoft.Extensions.Options.Options.Create(_options),
               _resourceName,
               string.IsNullOrWhiteSpace(tenantId) ? null : tenantId,
               _culture);
}
