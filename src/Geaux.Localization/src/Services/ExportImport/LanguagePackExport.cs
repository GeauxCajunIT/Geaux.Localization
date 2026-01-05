namespace Geaux.Localization.Services.ExportImport;

public sealed class LanguagePackExport
{
    public string Culture { get; set; } = default!;
    public Dictionary<string, string?> Translations { get; set; } = new();
}
