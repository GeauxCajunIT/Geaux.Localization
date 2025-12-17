namespace Geaux.Localization.Models;

/// <summary>
/// Represents a localized translation entry for a specific tenant, culture, and key.
/// </summary>
/// <remarks>
/// Use this class to store or retrieve localized text values for multi-tenant applications supporting multiple
/// cultures. Each instance uniquely identifies a translation by its tenant, culture, and key combination.
/// </remarks>
public class Translation
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant associated with the current context.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the culture name used for localization or formatting operations.
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the unique translation key associated with the current object.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized string value associated with this instance.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
