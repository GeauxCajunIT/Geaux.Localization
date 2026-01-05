namespace Geaux.Localization.Admin.DTOs;

public sealed class LocalizationKeyDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; }

    // ⭐ Used for grouping in the Keys page
    public string ModelName =>
        Key.Contains('.') ? Key.Split('.')[0] : "General";
}
