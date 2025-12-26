namespace Geaux.Localization.Models;

/// <summary>
/// Represents a localized value associated with a specific key, culture, and tenant scope.
/// </summary>
public sealed class LocalizationValue
{
    /// <summary>
    /// Gets or sets the primary key for the localized value.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated <see cref="LocalizationKey"/>.
    /// </summary>
    public int LocalizationKeyId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent <see cref="LocalizationKey"/>.
    /// </summary>
    public LocalizationKey LocalizationKey { get; set; } = default!;

    /// <summary>
    /// Gets or sets the culture identifier (for example, <c>en-US</c>) for the value.
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the tenant identifier that scopes this value. Null represents the global tenant.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the localized text.
    /// </summary>
    public string Value { get; set; } = string.Empty;

}
