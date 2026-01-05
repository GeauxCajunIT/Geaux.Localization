namespace Geaux.Localization.Admin.DTOs;

public sealed class LocalizationCultureDto
{
    public int Id { get; set; }
    public string CultureCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsRightToLeft { get; set; }
    public bool RequiresReview { get; set; }
    public string? FlagCode { get; set; }
}

