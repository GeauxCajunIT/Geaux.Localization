namespace Geaux.Localization.Models;

/// <summary>
/// Backwards-compatible DTO for older code paths. Not mapped by EF Core in v1.4+.
/// </summary>
[Obsolete("Translation is deprecated. Use LocalizationKey and LocalizationValue instead.")]
public sealed class Translation
{
    public int Id { get; set; }
    public string? TenantId { get; set; }
    public string Culture { get; set; } = "en-US";
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
