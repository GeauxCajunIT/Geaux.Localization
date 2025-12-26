namespace Geaux.Localization.Models;

/// <summary>
/// Backwards-compatible DTO for older code paths. Not mapped by EF Core in v1.4+.
/// </summary>
[Obsolete("Translation is deprecated. Use LocalizationKey and LocalizationValue instead.")]
public sealed class Translation
{
    /// <summary>
    /// Gets or sets the primary key for the translation record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier associated with the translation. Null for global scope.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the culture of the translation (for example, <c>en-US</c>).
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the localization key for the translation.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
