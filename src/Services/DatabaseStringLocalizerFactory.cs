using Geaux.Localization.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.Services;

/// <summary>
/// Provides an <see cref="IStringLocalizerFactory"/> implementation that creates localizers using database-backed
/// resources, scoped to the current tenant and culture as determined by the HTTP context.
/// </summary>
/// <remarks>
/// This factory is designed for multi-tenant, multi-culture applications where localization context is resolved per
/// request. All created localizers use tenant and culture information from the current HTTP context, ignoring resource
/// type, base name, or location parameters. Use this class when you need localization that adapts dynamically to the
/// current request's tenant and culture.
/// </remarks>
public class DatabaseStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly GeauxLocalizationContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseStringLocalizerFactory"/> class.
    /// </summary>
    /// <param name="db">The localization database context used to create <see cref="DatabaseStringLocalizer"/> instances.</param>
    /// <param name="httpContextAccessor">The accessor used to retrieve the current HTTP context.</param>
    public DatabaseStringLocalizerFactory(GeauxLocalizationContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Creates an <see cref="IStringLocalizer"/> instance that provides localized strings for the current tenant and culture.
    /// </summary>
    /// <remarks>
    /// The returned localizer always uses the tenant and culture information from the current HTTP context, regardless of
    /// the value of <paramref name="resourceSource"/>. This method is suitable for multi-tenant, multi-culture applications
    /// where localization context is determined per request.
    /// </remarks>
    /// <param name="resourceSource">The type that represents the resource source. This parameter is ignored.</param>
    /// <returns>An <see cref="IStringLocalizer"/> configured for the tenant and culture associated with the current HTTP request.</returns>
    public IStringLocalizer Create(Type resourceSource)
    {
        // Just ignore resourceSource, we always resolve tenant/culture from HttpContext
        return new DatabaseStringLocalizer(_db, ResolveTenantId(), ResolveCulture());
    }

    /// <summary>
    /// Creates a new instance of an <see cref="IStringLocalizer"/> for the current tenant and culture.
    /// </summary>
    /// <remarks>
    /// The returned localizer is always scoped to the current tenant and culture, regardless of the values provided for
    /// <paramref name="baseName"/> and <paramref name="location"/>.
    /// </remarks>
    /// <param name="baseName">The base name of the resource to localize. This parameter is ignored.</param>
    /// <param name="location">The location of the resource to localize. This parameter is ignored.</param>
    /// <returns>An <see cref="IStringLocalizer"/> instance configured for the current tenant and culture.</returns>
    public IStringLocalizer Create(string baseName, string location)
    {
        // Same logic, baseName/location not used
        return new DatabaseStringLocalizer(_db, ResolveTenantId(), ResolveCulture());
    }

    /// <summary>
    /// Resolves the tenant identifier from the current HTTP context.
    /// </summary>
    /// <returns>The current tenant identifier or "DefaultTenant" when it cannot be determined.</returns>
    private string ResolveTenantId()
    {
        return _httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString() ?? "DefaultTenant";
    }

    /// <summary>
    /// Resolves the culture from the current HTTP context.
    /// </summary>
    /// <returns>The current request culture name or "en-US" when it cannot be determined.</returns>
    private string ResolveCulture()
    {
        var feature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
        return feature?.RequestCulture.Culture.Name ?? "en-US";
    }
}
