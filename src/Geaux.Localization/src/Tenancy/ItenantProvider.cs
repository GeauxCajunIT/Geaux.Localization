namespace Geaux.Localization.Tenancy;

/// <summary>
/// Provides the current tenant identifier for multi-tenant localization.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Returns the current tenant ID, or null for global context.
    /// </summary>
    string? GetTenantId();
}

