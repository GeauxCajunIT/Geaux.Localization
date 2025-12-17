namespace Geaux.Localization.Interfaces;

/// <summary>
/// Provides access to the identifier of the current tenant in a multi-tenant application context.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the identifier of the current tenant context.
    /// </summary>
    /// <remarks>
    /// Use this property to determine which tenant is currently active in multi-tenant scenarios. The value may be
    /// null or empty if no tenant context is set.
    /// </remarks>
    string CurrentTenantId { get; }
}
