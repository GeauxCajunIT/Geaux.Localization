using Geaux.Localization.Interfaces;

namespace Geaux.Localization.Services;

/// <summary>
/// Default <see cref="ITenantContext"/> implementation that returns no tenant (global translations).
/// </summary>
public sealed class NullTenantContext : ITenantContext
{
    /// <inheritdoc />
    public string? CurrentTenantId => null;
}
