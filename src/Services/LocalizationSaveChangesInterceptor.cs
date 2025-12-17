// // <copyright file="" company="GeauxCajunIT">
// // Copyright (c) GeauxCajunIT. All rights reserved.
// // </copyright>

using Geaux.Localization.Attributes;
using Geaux.Localization.Data;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Geaux.Localization.Services;

/// <summary>
/// Intercepts Entity Framework Core save operations to automatically upsert localized translations for entity
/// properties marked with the LocalizedAttribute.
/// </summary>
/// <remarks>This interceptor scans entities being added or modified for properties decorated with
/// LocalizedAttribute and ensures that their translations are present or updated in the localization database. It
/// supports multi-tenant scenarios by associating translations with a tenant identifier. This class is typically
/// registered with the DbContext to enable automatic localization management during SaveChanges operations.</remarks>
public class LocalizationSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly GeauxLocalizationContext _localizationDb;
    private readonly string _tenantId;


    /// <summary>
    /// Initializes a new instance of the LocalizationSaveChangesInterceptor class with the specified localization
    /// context and tenant identifier.
    /// </summary>
    /// <param name="localizationDb">The localization database context used to access and manage localization data.</param>
    /// <param name="tenantId">The unique identifier for the tenant associated with this interceptor. Cannot be null or empty.</param>
    public LocalizationSaveChangesInterceptor(GeauxLocalizationContext localizationDb, string tenantId)
    {
        _localizationDb = localizationDb;
        _tenantId = tenantId;
    }

    /// <summary>
    /// Intercepts the asynchronous save operation to upsert localized translations for entity properties marked with
    /// the LocalizedAttribute before changes are persisted to the database.
    /// </summary>
    /// <remarks>This method scans all added or modified entities in the DbContext for properties decorated
    /// with the LocalizedAttribute and ensures their translations are upserted into the localization store before the
    /// save operation completes. The method does not alter the save operation's outcome but may impact performance if
    /// many localized properties are present. If the DbContext is null, no action is taken.</remarks>
    /// <param name="eventData">Contextual information about the DbContext associated with the save operation. Must not be null.</param>
    /// <param name="result">The current interception result for the save operation, which can be used to suppress or modify the operation.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete. The default value is None.</param>
    /// <returns>A ValueTask containing the interception result for the save operation. The result is unchanged from the input
    /// parameter.</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var props = entry.Entity.GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(LocalizedAttribute)));

            foreach (var prop in props)
            {
                var attr = (LocalizedAttribute)Attribute.GetCustomAttribute(prop, typeof(LocalizedAttribute))!;
                var culture = attr.Culture ?? Thread.CurrentThread.CurrentCulture.Name;
                var value = prop.GetValue(entry.Entity)?.ToString() ?? string.Empty;

                // Main value
                await UpsertTranslation(_tenantId, culture, attr.Key, value, cancellationToken);

                // Error message
                if (!string.IsNullOrEmpty(attr.ErrorMessageKey))
                    await UpsertTranslation(_tenantId, culture, attr.ErrorMessageKey, $"{prop.Name} is required", cancellationToken);

                // Display name
                if (!string.IsNullOrEmpty(attr.DisplayNameKey))
                    await UpsertTranslation(_tenantId, culture, attr.DisplayNameKey, prop.Name, cancellationToken);

                // Display message (tooltip/info)
                if (!string.IsNullOrEmpty(attr.DisplayMessageKey))
                    await UpsertTranslation(_tenantId, culture, attr.DisplayMessageKey, $"Info about {prop.Name}", cancellationToken);
            }
        }

        await _localizationDb.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task UpsertTranslation(string tenantId, string culture, string key, string value, CancellationToken cancellationToken)
    {
        var existing = await _localizationDb.Translations
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Culture == culture && t.Key == key, cancellationToken);

        if (existing == null)
        {
            _localizationDb.Translations.Add(new Translation
            {
                TenantId = tenantId,
                Culture = culture,
                Key = key,
                Value = value
            });
        }
        else if (existing.Value != value)
        {
            existing.Value = value;
        }
    }
}

