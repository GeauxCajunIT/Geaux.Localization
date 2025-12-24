using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Geaux.Localization.Services;

/// <summary>
/// Seeds localization keys and default values for properties marked with <see cref="LocalizedAttribute"/> in the provided assemblies.
/// </summary>
/// <remarks>
/// This seeder ensures keys exist in <see cref="LocalizationKey"/> and that a value exists in <see cref="LocalizationValue"/>
/// for the provided culture/tenant.
/// </remarks>
public sealed class LocalizationSeeder
{
    private readonly GeauxLocalizationDbContext _db;
    private readonly IEnumerable<Assembly> _assemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationSeeder"/> class.
    /// </summary>
    /// <param name="db">Localization database context.</param>
    /// <param name="assemblies">Assemblies to scan for localized properties.</param>
    public LocalizationSeeder(GeauxLocalizationDbContext db, IEnumerable<Assembly> assemblies)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }

    /// <summary>
    /// Scans assemblies and creates missing keys/values for the specified culture.
    /// </summary>
    /// <param name="culture">Culture name (for example, <c>en-US</c>).</param>
    /// <param name="tenantId">Optional tenant identifier. If null/empty, seeds global translations.</param>
    /// <param name="overwriteSystemValues">When true, overwrites existing values that are marked <c>IsSystem</c>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SeedAsync(
        string culture,
        string? tenantId = null,
        bool overwriteSystemValues = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(culture))
            throw new ArgumentException("Culture must be provided.", nameof(culture));

        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;

        HashSet<string> keys = new HashSet<string>(StringComparer.Ordinal);

        foreach (Assembly assembly in _assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    LocalizedAttribute? attr = prop.GetCustomAttribute<LocalizedAttribute>();
                    if (attr == null) continue;

                    if (!string.IsNullOrWhiteSpace(attr.Key)) keys.Add(attr.Key);
                    if (!string.IsNullOrWhiteSpace(attr.ErrorMessageKey)) keys.Add(attr.ErrorMessageKey);
                    if (!string.IsNullOrWhiteSpace(attr.DisplayNameKey)) keys.Add(attr.DisplayNameKey);
                    if (!string.IsNullOrWhiteSpace(attr.DisplayMessageKey)) keys.Add(attr.DisplayMessageKey);
                }
            }
        }

        // Ensure keys exist
        List<string> existingKeys = await _db.LocalizationKeys
            .AsNoTracking()
            .Where(k => keys.Contains(k.Key))
            .Select(k => k.Key)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<string> missingKeys = keys.Except(existingKeys, StringComparer.Ordinal).ToList();
        foreach (var k in missingKeys)
        {
            _db.LocalizationKeys.Add(new LocalizationKey { Key = k });
        }

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Map key -> id
        Dictionary<string, int> keyIdMap = await _db.LocalizationKeys
            .AsNoTracking()
            .Where(k => keys.Contains(k.Key))
            .ToDictionaryAsync(k => k.Key, k => k.Id, cancellationToken)
            .ConfigureAwait(false);

        foreach (var key in keys)
        {
            var keyId = keyIdMap[key];

            LocalizationValue? existing = await _db.LocalizationValues
                .FirstOrDefaultAsync(v =>
                    v.LocalizationKeyId == keyId &&
                    v.Culture == culture &&
                    v.TenantId == tenantId, cancellationToken)
                .ConfigureAwait(false);

            if (existing == null)
            {
                _db.LocalizationValues.Add(new LocalizationValue
                {
                    LocalizationKeyId = keyId,
                    TenantId = tenantId,
                    Culture = culture,
                    Value = key
                });
            }
            else if (overwriteSystemValues && !string.Equals(existing.Value, key, StringComparison.Ordinal))
            {
                existing.Value = key;
            }
        }

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
