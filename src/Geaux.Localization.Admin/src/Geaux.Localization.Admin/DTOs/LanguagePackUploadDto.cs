namespace Geaux.Localization.Admin.DTOs;

public sealed class LanguagePackUploadDto
{
    public string CultureCode { get; set; } = string.Empty;
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
