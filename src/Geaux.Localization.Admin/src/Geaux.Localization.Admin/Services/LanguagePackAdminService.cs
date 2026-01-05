using Geaux.Localization.Admin.DTOs;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text.Json;

namespace Geaux.Localization.Admin.Services;

/// <summary>
/// Provides administrative operations for exporting and importing language packs.
/// Supports JSON, CSV, and ZIP formats, enabling bulk localization workflows
/// for contributors and automated systems.
/// </summary>
public sealed class LanguagePackAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;
    private readonly IActivityLogRepository _activity;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguagePackAdminService"/>.
    /// </summary>
    /// <param name="factory">The database context factory.</param>
    /// <param name="activity">The activity log repository.</param>
    public LanguagePackAdminService(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IActivityLogRepository activity)
    {
        _factory = factory;
        _activity = activity;
    }

    // --------------------------------------------------------------------
    // EXPORTS
    // --------------------------------------------------------------------

    /// <summary>
    /// Exports a single culture as a JSON language pack.
    /// </summary>
    /// <param name="culture">The culture code.</param>
    /// <returns>A UTF‑8 encoded JSON byte array.</returns>
    public async Task<byte[]> ExportCultureJsonAsync(string culture)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var rows = await db.LocalizationValues
            .Where(v => v.Culture == culture)
            .Select(v => new
            {
                key = v.LocalizationKey.Key,
                value = v.Value
            })
            .OrderBy(v => v.key)
            .ToListAsync();

        await _activity.AddAsync(ActivityLogFactory.LanguagePackExported(culture));

        return JsonSerializer.SerializeToUtf8Bytes(rows, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Exports all cultures as a ZIP archive containing one JSON file per culture.
    /// </summary>
    /// <returns>A byte array representing the ZIP archive.</returns>
    public async Task<byte[]> ExportAllCulturesZipAsync()
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        List<string> cultures = await db.LocalizationCultures
            .OrderBy(c => c.CultureCode)
            .Select(c => c.CultureCode)
            .ToListAsync();

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var culture in cultures)
            {
                var json = await ExportCultureJsonAsync(culture);
                ZipArchiveEntry entry = zip.CreateEntry($"{culture}.json");

                using Stream entryStream = entry.Open();
                await entryStream.WriteAsync(json);
            }
        }

        await _activity.AddAsync(ActivityLogFactory.LanguagePackExported("ALL"));

        return ms.ToArray();
    }

    // --------------------------------------------------------------------
    // IMPORTS
    // --------------------------------------------------------------------

    /// <summary>
    /// Imports a JSON language pack for a single culture.
    /// </summary>
    /// <param name="dto">The upload DTO containing culture and file stream.</param>
    public async Task ImportCultureJsonAsync(LanguagePackUploadDto dto)
    {
        List<JsonRow> items = await JsonSerializer.DeserializeAsync<List<JsonRow>>(dto.FileStream)
                    ?? new();

        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        foreach (JsonRow row in items)
        {
            LocalizationKey key = await db.LocalizationKeys.FirstOrDefaultAsync(k => k.Key == row.key)
                ?? (db.LocalizationKeys.Add(new LocalizationKey
                {
                    Key = row.key,
                    IsSystem = false
                }).Entity);

            await db.SaveChangesAsync();

            LocalizationValue? value = await db.LocalizationValues.FirstOrDefaultAsync(v =>
                v.LocalizationKeyId == key.Id &&
                v.Culture == dto.CultureCode);

            if (value is null)
            {
                db.LocalizationValues.Add(new LocalizationValue
                {
                    LocalizationKeyId = key.Id,
                    Culture = dto.CultureCode,
                    Value = row.value
                });
            }
            else
            {
                value.Value = row.value;
            }
        }

        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.LanguagePackImported(dto.CultureCode)
        );
    }

    /// <summary>
    /// Imports a ZIP archive containing multiple JSON language packs.
    /// </summary>
    /// <param name="stream">The ZIP stream.</param>
    public async Task ImportZipAsync(Stream stream)
    {
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

        foreach (ZipArchiveEntry entry in zip.Entries)
        {
            if (!entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            var culture = Path.GetFileNameWithoutExtension(entry.FullName);

            using Stream entryStream = entry.Open();

            var dto = new LanguagePackUploadDto
            {
                CultureCode = culture,
                FileStream = entryStream,
                FileName = entry.FullName
            };

            await ImportCultureJsonAsync(dto);
        }

        await _activity.AddAsync(ActivityLogFactory.LanguagePackImported("ZIP"));
    }

    // --------------------------------------------------------------------
    // PREVIEW
    // --------------------------------------------------------------------

    /// <summary>
    /// Parses a JSON language pack into preview rows without saving.
    /// </summary>
    /// <param name="jsonText">The JSON text.</param>
    /// <returns>A list of preview rows.</returns>
    public List<LanguagePackPreviewRow> PreviewJson(string jsonText)
    {
        List<JsonRow> rows = JsonSerializer.Deserialize<List<JsonRow>>(jsonText)
                   ?? new();

        return rows.Select(r => new LanguagePackPreviewRow(r.key, r.value)).ToList();
    }

    // --------------------------------------------------------------------
    // INTERNAL MODELS
    // --------------------------------------------------------------------

    private sealed record JsonRow(string key, string value);

    /// <summary>
    /// Represents a preview row for a language pack import.
    /// </summary>
    public sealed record LanguagePackPreviewRow(string Key, string Value);
}
