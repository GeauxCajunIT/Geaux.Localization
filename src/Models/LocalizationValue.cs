namespace Geaux.Localization.Models;

public sealed class LocalizationValue
{
    public int Id { get; set; }

    public int LocalizationKeyId { get; set; }
    public LocalizationKey LocalizationKey { get; set; } = default!;

    public string Culture { get; set; } = "en-US";
    public string? TenantId { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsSystem { get; internal set; }
}
