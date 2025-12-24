using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Geaux.Localization.SampleBlazor.Services;

public static class LocalizedAttributeSeeder
{
    public static async Task SeedAsync(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IEnumerable<Type> modelTypes,
        IEnumerable<string> supportedCultures,
        string? tenantId = null,
        CancellationToken ct = default)
    {
        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync(ct);

        string[] cultures = supportedCultures
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (Type modelType in modelTypes)
        {
            foreach (PropertyInfo prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                LocalizedAttribute? attr = prop.GetCustomAttribute<LocalizedAttribute>();
                if (attr is null) continue;

                // Determine cultures to seed for this attribute
                string[] culturesForAttr = !string.IsNullOrWhiteSpace(attr.Culture)
                    ? new[] { attr.Culture!.Trim() }
                    : cultures;

                // Collect keys to seed (unique)
                HashSet<string> keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(attr.Key))
                    keys.Add(attr.Key);

                if (!string.IsNullOrWhiteSpace(attr.DisplayNameKey))
                    keys.Add(attr.DisplayNameKey!);

                if (!string.IsNullOrWhiteSpace(attr.DisplayMessageKey))
                    keys.Add(attr.DisplayMessageKey!);

                if (!string.IsNullOrWhiteSpace(attr.ErrorMessageKey))
                    keys.Add(attr.ErrorMessageKey!);

                foreach (string? culture in culturesForAttr)
                {
                    foreach (string key in keys)
                    {
                        // Default seed value: last segment or property name (editable later in admin)
                        string defaultValue = MakeDefaultValue(modelType, prop, key);

                        await UpsertAsync(
                            db,
                            tenantId,
                            culture,
                            key,
                            defaultValue,
                            overwrite: true,
                            ct);
                    }
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task UpsertAsync(
    GeauxLocalizationDbContext db,
    string? tenantId,
    string culture,
    string key,
    string value,
    bool overwrite,
    CancellationToken ct)
    {
        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;

        // 1. Ensure key exists
        LocalizationKey k = await db.LocalizationKeys
            .FirstOrDefaultAsync(x => x.Key == key, ct)
            ?? db.LocalizationKeys.Add(new LocalizationKey
            {
                Key = key,
                IsSystem = true
            }).Entity;

        await db.SaveChangesAsync(ct); // ensure key has Id

        // 2. Upsert value
        LocalizationValue? existing = await db.LocalizationValues.FirstOrDefaultAsync(v =>
            v.LocalizationKeyId == k.Id &&
            v.Culture == culture &&
            v.TenantId == tenantId,
            ct);

        if (existing == null)
        {
            db.LocalizationValues.Add(new LocalizationValue
            {
                LocalizationKeyId = k.Id,
                Culture = culture,
                TenantId = tenantId,
                Value = value
            });
            return;
        }

        if (overwrite || string.IsNullOrWhiteSpace(existing.Value))
        {
            existing.Value = value;
        }
    }



    private static string MakeDefaultValue(Type modelType, PropertyInfo prop, string key)
    {
        // Prefer a readable default:
        // - if key ends with ".Display" or ".Name" etc, use property name
        // - else use the key's last segment as a starter
        string? last = key.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (string.IsNullOrWhiteSpace(last))
            return prop.Name;

        // common patterns
        if (last.Equals("Display", StringComparison.OrdinalIgnoreCase) ||
            last.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
            last.Equals("Label", StringComparison.OrdinalIgnoreCase))
            return prop.Name;

        // If they used the required Key as a semantic identifier, use last segment
        return last;
    }
}
