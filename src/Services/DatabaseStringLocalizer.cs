using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using LocalizationOptions = Geaux.Localization.Config.LocalizationOptions;

namespace Geaux.Localization.Services;

public sealed class DatabaseStringLocalizer : IStringLocalizer
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _dbFactory;
    private readonly LocalizationOptions _options;
    private readonly string _resourceName;
    private readonly string? _tenantId;
    private readonly CultureInfo _culture;

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

    public LocalizedString this[string name]
        => GetString(name);

    public LocalizedString this[string name, params object[] arguments]
        => GetString(name, arguments);

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

    public DatabaseStringLocalizer WithCulture(CultureInfo culture)
        => new(_dbFactory,
               Microsoft.Extensions.Options.Options.Create(_options),
               _resourceName,
               _tenantId,
               culture);

    public DatabaseStringLocalizer WithTenant(string? tenantId)
        => new(_dbFactory,
               Microsoft.Extensions.Options.Options.Create(_options),
               _resourceName,
               string.IsNullOrWhiteSpace(tenantId) ? null : tenantId,
               _culture);
}
