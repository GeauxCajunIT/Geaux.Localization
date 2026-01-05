namespace Geaux.Localization.EFCore.Seeding;

/// <summary>
/// Backward-compatible wrapper for the new LocalizationSeeder.
/// This restores the public API expected by tests and sample apps,
/// while delegating all logic to the new internal implementation.
/// </summary>
public sealed class KeySeeder
{
    private readonly LocalizationSeeder _inner;

    /// <summary>
    /// Restores the original constructor signature.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan for LocalizedAttribute.</param>
    public KeySeeder(IEnumerable<Assembly> assemblies)
    {
        _inner = new LocalizationSeeder(assemblies);
    }

    /// <summary>
    /// Backward-compatible SeedAsync wrapper.
    /// Delegates to the new LocalizationSeeder implementation.
    /// </summary>
    public Task SeedAsync(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IEnumerable<Type> modelTypes,
        IEnumerable<string> supportedCultures,
        string? tenantId = null,
        bool overwriteExisting = false,
        CancellationToken ct = default)
    {
        return _inner.SeedAsync(factory, modelTypes, supportedCultures, tenantId, overwriteExisting, ct);
    }

    // ----------------------------------------------------------------------
    // NEW INTERNAL IMPLEMENTATION (unchanged from your refactor)
    // ----------------------------------------------------------------------
    public sealed class LocalizationSeeder
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public LocalizationSeeder(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

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

            // Determine types to scan
            List<Type> typesToScan = modelTypes.Where(t => t is not null).Distinct().ToList();
            if (typesToScan.Count == 0)
            {
                foreach (Assembly a in _assemblies)
                {
                    Type[] types;
                    try { types = a.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray(); }

                    typesToScan.AddRange(types);
                }

                typesToScan = typesToScan.Distinct().ToList();
            }

            // Collect keys from LocalizedAttribute
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
                            Value = key // default fallback
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
}
