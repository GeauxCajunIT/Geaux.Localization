namespace Geaux.Localization.Admin.DTOs;

public sealed class LocalizationValueDto
{
    public int KeyId { get; set; }
    public string CultureCode { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    // Optional convenience for grouping
    public string Key { get; set; } = string.Empty;
    public string ModelName =>
        Key.Contains('.') ? Key.Split('.')[0] : "General";
}
