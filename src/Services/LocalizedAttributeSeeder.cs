using Geaux.Localization.Attributes;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Geaux.Localization.Services;

public static class LocalizedAttributeSeeder
{
    public static async Task SeedAsync(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IEnumerable<Type> modelTypes,
        IEnumerable<string> supportedCultures,
        string? tenantId = null,
        bool overwrite = false,
        CancellationToken ct = default)
    {
        await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync(ct);

        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;

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

                string[] culturesForAttr = !string.IsNullOrWhiteSpace(attr.Culture)
                    ? new[] { attr.Culture!.Trim() }
                    : cultures;

                HashSet<string> keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(attr.Key)) keys.Add(attr.Key);
                if (!string.IsNullOrWhiteSpace(attr.DisplayNameKey)) keys.Add(attr.DisplayNameKey!);
                if (!string.IsNullOrWhiteSpace(attr.DisplayMessageKey)) keys.Add(attr.DisplayMessageKey!);
                if (!string.IsNullOrWhiteSpace(attr.ErrorMessageKey)) keys.Add(attr.ErrorMessageKey!);

                foreach (var culture in culturesForAttr)
                    foreach (var key in keys)
                    {
                        var defaultValue = MakeDefaultValue(prop, key);
                        await UpsertAsync(db, tenantId, culture, key, defaultValue, overwrite, ct);
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
        LocalizationKey k = await db.LocalizationKeys.FirstOrDefaultAsync(x => x.Key == key, ct)
            ?? db.LocalizationKeys.Add(new LocalizationKey { Key = key, IsSystem = true }).Entity;

        await db.SaveChangesAsync(ct); // ensure key Id

        LocalizationValue? existing = await db.LocalizationValues.FirstOrDefaultAsync(v =>
            v.LocalizationKeyId == k.Id &&
            v.Culture == culture &&
            v.TenantId == tenantId, ct);

        if (existing is null)
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

        if (overwrite)
            existing.Value = value;
    }

    private static string MakeDefaultValue(PropertyInfo prop, string key)
    {
        var last = key.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (string.IsNullOrWhiteSpace(last)) return prop.Name;

        if (last.Equals("Display", StringComparison.OrdinalIgnoreCase) ||
            last.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
            last.Equals("Label", StringComparison.OrdinalIgnoreCase))
            return prop.Name;

        return last;
    }
}
