using Microsoft.AspNetCore.Routing;

namespace Geaux.Localization.Admin.Extensions;

/// <summary>
/// Provides extension methods for configuring GeauxLocalization administrative endpoints on an endpoint route builder.
/// </summary>
public static class GeauxLocalizationEndpointExtensions
{
    /// <summary>
    /// Adds the Geaux Localization admin dashboard endpoint to the specified endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to which the admin dashboard endpoint will be added.</param>
    /// <returns>The endpoint route builder with the admin dashboard endpoint configured.</returns>
    public static IEndpointRouteBuilder MapGeauxLocalizationAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // endpoints.MapRazorComponents<GeauxLocalizationAdminMarker>();
        return endpoints;
    }
}
