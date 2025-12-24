namespace Geaux.Localization.Interfaces;

/// <summary>
/// Provides access to the current tenant identifier for localization scoping.
/// </summary>
/// <remarks>
/// If your application is not multi-tenant, you can use a null/empty tenant identifier.
/// </remarks>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant identifier, or null/empty for global translations.
    /// </summary>
    string? CurrentTenantId { get; }
}
