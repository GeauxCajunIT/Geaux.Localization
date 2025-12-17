// // <copyright file="" company="GeauxCajunIT">
// // Copyright (c) GeauxCajunIT. All rights reserved.
// // </copyright>

using Geaux.Localization.Attributes;
using Geaux.Localization.Data;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Geaux.Localization.Services;

/// <summary>
/// Provides functionality to seed localization translation entries for properties marked with the LocalizedAttribute
/// across specified assemblies.
/// </summary>
/// <remarks>This class is typically used during application setup or tenant initialization to ensure that all
/// required translation keys exist in the database for a given tenant and culture. It scans the provided assemblies for
/// properties decorated with the LocalizedAttribute and creates default translation entries if they do not already
/// exist. This helps prevent missing translation keys at runtime.</remarks>
public class LocalizationSeeder
{
    private readonly GeauxLocalizationContext _db;
    private readonly IEnumerable<Assembly> _assemblies;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="db"></param>
    /// <param name="assemblies"></param>
    public LocalizationSeeder(GeauxLocalizationContext db, IEnumerable<Assembly> assemblies)
    {
        _db = db;
        _assemblies = assemblies;
    }

    /// <summary>
    /// Seeds default translation entries for all localized properties for the specified tenant and culture if they do
    /// not already exist.
    /// </summary>
    /// <remarks>This method scans all loaded assemblies for properties marked with the LocalizedAttribute and
    /// ensures that a default translation entry exists for each property for the given tenant and culture. If an entry
    /// does not exist, it is created with the property name as the default value. Existing translations are not
    /// overwritten.</remarks>
    /// <param name="tenantId">The unique identifier of the tenant for which to seed translations. Cannot be null or empty.</param>
    /// <param name="defaultCulture">The culture code (for example, "en-US") to use when seeding default translations. Defaults to "en-US" if not
    /// specified.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    public async Task SeedAsync(string tenantId, string defaultCulture = "en-US", CancellationToken cancellationToken = default)
    {
        var localizedProps = _assemblies
            .SelectMany(a => a.GetTypes())
            .SelectMany(t => t.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(LocalizedAttribute)))
                .Select(p => (Type: t, Property: p, Attr: (LocalizedAttribute)Attribute.GetCustomAttribute(p, typeof(LocalizedAttribute))!)));

        foreach ((var type, var prop, var attr) in localizedProps)
        {
            var key = attr.Key ?? $"{type.Name}.{prop.Name}";

            var exists = await _db.Translations
                .Where(t => t.TenantId == tenantId && t.Culture == defaultCulture && t.Key == key)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                _db.Translations.Add(new Translation
                {
                    TenantId = tenantId,
                    Culture = defaultCulture,
                    Key = key,
                    Value = prop.Name // fallback default
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}

