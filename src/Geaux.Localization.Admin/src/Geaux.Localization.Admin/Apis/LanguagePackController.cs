using Geaux.Localization.Services.ExportImport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("admin/localization/api/language-pack")]
public sealed class LanguagePackController : ControllerBase
{
    private readonly LocalizationExportService _export;
    private readonly LocalizationImportService _import;

    public LanguagePackController(
        LocalizationExportService export,
        LocalizationImportService import)
    {
        _export = export;
        _import = import;
    }

    // ------------------------------------------------------------
    // EXPORT: JSON (single culture)
    // ------------------------------------------------------------
    [HttpGet("{culture}")]
    public async Task<IActionResult> GetJsonForCulture(
        string culture,
        [FromQuery] string? tenantId = null)
    {
        (List<Geaux.Localization.Models.LocalizationKey>? keys, List<Geaux.Localization.Models.LocalizationValue>? values) = await _export.GetKeysAndValuesForCultureAsync(culture, tenantId);

        Dictionary<string, string> dict = _export.GenerateJsonForCulture(culture, keys, values);

        string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", $"{culture}.json");
    }

    // ------------------------------------------------------------
    // EXPORT: CSV (single culture)
    // ------------------------------------------------------------
    [HttpGet("{culture}/csv")]
    public async Task<IActionResult> GetCsvForCulture(
        string culture,
        [FromQuery] string? tenantId = null)
    {
        (List<Geaux.Localization.Models.LocalizationKey>? keys, List<Geaux.Localization.Models.LocalizationValue>? values) = await _export.GetKeysAndValuesForCultureAsync(culture, tenantId);

        string csv = _export.GenerateCsvForCulture(culture, keys, values);
        byte[] bytes = Encoding.UTF8.GetBytes(csv);

        return File(bytes, "text/csv", $"{culture}.csv");
    }

    // ------------------------------------------------------------
    // EXPORT: Missing-only CSV
    // ------------------------------------------------------------
    [HttpGet("{culture}/missing")]
    public async Task<IActionResult> GetMissingCsvForCulture(
        string culture,
        [FromQuery] string? tenantId = null)
    {
        (List<Geaux.Localization.Models.LocalizationKey>? keys, List<Geaux.Localization.Models.LocalizationValue>? values) = await _export.GetKeysAndValuesForCultureAsync(culture, tenantId);

        string csv = _export.GenerateMissingCsvForCulture(culture, keys, values);
        byte[] bytes = Encoding.UTF8.GetBytes(csv);

        return File(bytes, "text/csv", $"{culture}-missing.csv");
    }

    // ------------------------------------------------------------
    // EXPORT: Missing-only JSON
    // ------------------------------------------------------------
    [HttpGet("{culture}/missing.json")]
    public async Task<IActionResult> GetMissingJsonForCulture(
        string culture,
        [FromQuery] string? tenantId = null)
    {
        (List<Geaux.Localization.Models.LocalizationKey>? keys, List<Geaux.Localization.Models.LocalizationValue>? values) = await _export.GetKeysAndValuesForCultureAsync(culture, tenantId);

        Dictionary<string, string> dict = _export.GenerateMissingJsonForCulture(culture, keys, values);

        string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", $"{culture}-missing.json");
    }

    // ------------------------------------------------------------
    // EXPORT: ZIP (all cultures)
    // ------------------------------------------------------------
    [HttpGet("all")]
    public async Task<IActionResult> GetZipForAllCultures(
        [FromQuery] string? tenantId = null)
    {
        byte[] zipBytes = await _export.GenerateZipForAllCulturesAsync(tenantId);

        return File(zipBytes, "application/zip", "language-packs.zip");
    }

    // ------------------------------------------------------------
    // IMPORT: JSON
    // ------------------------------------------------------------
    [HttpPost("upload/json/{culture}")]
    public async Task<IActionResult> UploadJson(
        string culture,
        IFormFile file,
        [FromQuery] string? tenantId = null)
    {
        using Stream stream = file.OpenReadStream();
        ImportResult result = await _import.ImportJsonAsync(culture, stream, tenantId);
        return Ok(result);
    }

    // ------------------------------------------------------------
    // IMPORT: CSV
    // ------------------------------------------------------------
    [HttpPost("upload/csv/{culture}")]
    public async Task<IActionResult> UploadCsv(
        string culture,
        IFormFile file,
        [FromQuery] string? tenantId = null)
    {
        using Stream stream = file.OpenReadStream();
        ImportResult result = await _import.ImportCsvAsync(culture, stream, tenantId);
        return Ok(result);
    }

    // ------------------------------------------------------------
    // IMPORT: ZIP (multiple cultures)
    // ------------------------------------------------------------
    [HttpPost("upload/zip")]
    public async Task<IActionResult> UploadZip(
        IFormFile file,
        [FromQuery] string? tenantId = null)
    {
        using Stream stream = file.OpenReadStream();
        ImportResult result = await _import.ImportZipAsync(stream, tenantId);
        return Ok(result);
    }
}
