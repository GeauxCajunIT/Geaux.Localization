using Geaux.Localization.Data;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.Services;

/// <summary>
/// Provides string localization by retrieving localized resources from a database for a specific tenant and culture.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IStringLocalizer"/> interface to enable multi-tenant, culture-specific
/// localization using a database as the backing store. It is typically used to support dynamic or runtime localization
/// scenarios where translations are managed outside of static resource files. Instances are scoped to a particular
/// tenant and culture and do not fall back to parent cultures automatically.
/// </remarks>
public class DatabaseStringLocalizer : IStringLocalizer
{
    private readonly GeauxLocalizationContext _db;
    private readonly string _tenantId;
    private readonly string _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseStringLocalizer"/> class.
    /// </summary>
    /// <param name="db">The localization database context used to read translations.</param>
    /// <param name="tenantId">The tenant identifier associated with the current localization scope.</param>
    /// <param name="culture">The culture identifier used to select the correct translations.</param>
    public DatabaseStringLocalizer(GeauxLocalizationContext db, string tenantId, string culture)
    {
        _db = db;
        _tenantId = tenantId;
        _culture = culture;
    }

    /// <summary>
    /// Gets the localized string associated with the specified resource name for the current tenant and culture.
    /// </summary>
    /// <remarks>
    /// If a translation for the specified resource name does not exist for the current tenant and culture, the
    /// returned <see cref="LocalizedString"/> will have its <c>ResourceNotFound</c> property set to
    /// <see langword="true"/> and its value will be the resource name itself.
    /// </remarks>
    /// <param name="name">The name of the resource to retrieve the localized string for.</param>
    /// <returns>
    /// A <see cref="LocalizedString"/> containing the localized value if found; otherwise, a <see cref="LocalizedString"/>
    /// containing the resource name as the value and indicating that the resource was not found.
    /// </returns>
    public LocalizedString this[string name]
    {
        get
        {
            var translation = _db.Translations
                .FirstOrDefault(t => t.TenantId == _tenantId && t.Culture == _culture && t.Key == name);

            if (translation != null)
            {
                return new LocalizedString(name, translation.Value, false);
            }

            return new LocalizedString(name, name, true);
        }
    }

    /// <summary>
    /// Gets the localized string resource for the specified name, formatted with the provided arguments if applicable.
    /// </summary>
    /// <param name="name">The name of the string resource to retrieve. Cannot be null.</param>
    /// <param name="arguments">An array of objects to format the string with, if the resource supports formatting.</param>
    /// <returns>A <see cref="LocalizedString"/> representing the localized resource.</returns>
    public LocalizedString this[string name, params object[] arguments] =>
        this[name];

    /// <summary>
    /// Returns all localized strings for the current culture and tenant.
    /// </summary>
    /// <param name="includeParentCultures">True to include strings from parent cultures; otherwise, false.</param>
    /// <returns>
    /// An enumerable collection of <see cref="LocalizedString"/> objects representing all available localized strings
    /// for the current culture and tenant.
    /// </returns>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _db.Translations
            .Where(t => t.TenantId == _tenantId && t.Culture == _culture)
            .Select(t => new LocalizedString(t.Key, t.Value, false));
    }
}
