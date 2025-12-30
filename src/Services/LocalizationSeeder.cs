using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Geaux.Localization.Services;

/// <summary>
/// Seeds localization keys and default values for properties marked with <see cref="LocalizedAttribute"/>.
/// </summary>
/// <remarks>
/// This seeder ensures keys exist in <see cref="LocalizationKey"/> and that values exist in
/// <see cref="LocalizationValue"/> for each requested culture/tenant.
/// Uses the normalized schema: LocalizationKeys + LocalizationValues.
/// </remarks>
public sealed class LocalizationSeeder
{
    private readonly IEnumerable<Assembly> _assemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationSeeder"/> class.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan for localized properties.</param>
    public LocalizationSeeder(IEnumerable<Assembly> assemblies)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }

    /// <summary>
    /// Scans assemblies (and/or provided model types) and creates missing keys/values for the specified cultures.
    /// </summary>
    /// <param name="factory">DbContext factory.</param>
    /// <param name="modelTypes">
    /// Optional: model types to scan. If empty, the configured assemblies are scanned.
    /// </param>
    /// <param name="supportedCultures">Cultures to seed (e.g., en-US, fr-FR).</param>
    /// <param name="tenantId">Optional tenant identifier. If null/empty, seeds global translations.</param>
    /// <param name="overwriteExisting">When true, overwrites existing values.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task SeedAsync(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IEnumerable<Type> modelTypes,
        IEnumerable<string> supportedCultures,
        string? tenantId = null,
        bool overwriteExisting = false,
        CancellationToken ct = default)
    {
        if (factory is null) throw new ArgumentNullException(nameof(factory));
        if (supportedCultures is null) throw new ArgumentNullException(nameof(supportedCultures));
        if (modelTypes is null) throw new ArgumentNullException(nameof(modelTypes));

        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;

        string[] cultures = supportedCultures
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (cultures.Length == 0)
            throw new ArgumentException("At least one culture must be provided.", nameof(supportedCultures));

        // Decide what we scan: explicit modelTypes OR assemblies
        List<Type> typesToScan = modelTypes.Where(t => t is not null).Distinct().ToList();
        if (typesToScan.Count == 0)
        {
            foreach (Assembly a in _assemblies)
            {
                // ignore reflection load failures as best-effort seeding
                Type[] types;
                try { types = a.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray(); }

                typesToScan.AddRange(types);
            }

            typesToScan = typesToScan.Distinct().ToList();
        }

        // Collect all keys from attributes
        HashSet<string> keys = new(StringComparer.OrdinalIgnoreCase);

        foreach (Type type in typesToScan)
        {
            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                LocalizedAttribute? attr = prop.GetCustomAttribute<LocalizedAttribute>();
                if (attr is null) continue;

                if (!string.IsNullOrWhiteSpace(attr.Key)) keys.Add(attr.Key);
                if (!string.IsNullOrWhiteSpace(attr.DisplayNameKey)) keys.Add(attr.DisplayNameKey);
                if (!string.IsNullOrWhiteSpace(attr.DisplayMessageKey)) keys.Add(attr.DisplayMessageKey);
                if (!string.IsNullOrWhiteSpace(attr.ErrorMessageKey)) keys.Add(attr.ErrorMessageKey);
            }
        }

        if (keys.Count == 0)
            return;

        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync(ct);

        // Ensure keys exist
        List<string> existingKeys = await db.LocalizationKeys
            .AsNoTracking()
            .Where(k => keys.Contains(k.Key))
            .Select(k => k.Key)
            .ToListAsync(ct);

        List<string> missingKeys = keys
            .Except(existingKeys, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (string k in missingKeys)
        {
            db.LocalizationKeys.Add(new LocalizationKey
            {
                Key = k,
                IsSystem = true
            });
        }

        if (missingKeys.Count > 0)
            await db.SaveChangesAsync(ct);

        // Map key -> id
        Dictionary<string, int> keyIdMap = await db.LocalizationKeys
            .AsNoTracking()
            .Where(k => keys.Contains(k.Key))
            .ToDictionaryAsync(k => k.Key, k => k.Id, StringComparer.OrdinalIgnoreCase, ct);

        // Ensure values exist for each culture
        foreach (string culture in cultures)
        {
            foreach (string key in keys)
            {
                int keyId = keyIdMap[key];

                LocalizationValue? existing = await db.LocalizationValues
                    .FirstOrDefaultAsync(v =>
                        v.LocalizationKeyId == keyId &&
                        v.Culture == culture &&
                        v.TenantId == tenantId,
                        ct);

                if (existing is null)
                {
                    db.LocalizationValues.Add(new LocalizationValue
                    {
                        LocalizationKeyId = keyId,
                        TenantId = tenantId,
                        Culture = culture,
                        Value = key // default fallback value
                    });
                }
                else if (overwriteExisting)
                {
                    existing.Value = key;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
