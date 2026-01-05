using Geaux.Localization.Admin.DTOs;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Geaux.Localization.Admin.Services;

/// <summary>
/// Provides administrative operations for managing localization values.
/// This includes retrieving values, updating values, and performing
/// bulk import/export operations in CSV or JSON formats.
/// </summary>
public sealed class LocalizationValueAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;
    private readonly IActivityLogRepository _activity;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationValueAdminService"/>.
    /// </summary>
    /// <param name="factory">The database context factory.</param>
    /// <param name="activity">The activity log repository.</param>
    public LocalizationValueAdminService(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IActivityLogRepository activity)
    {
        _factory = factory;
        _activity = activity;
    }

    /// <summary>
    /// Retrieves all localization values for a specific culture.
    /// </summary>
    /// <param name="culture">The culture code.</param>
    /// <returns>A list of <see cref="LocalizationValueDto"/>.</returns>
    public async Task<List<LocalizationValueDto>> GetValuesForCultureAsync(string culture)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        return await db.LocalizationValues
            .Where(v => v.Culture == culture)
            .Select(v => new LocalizationValueDto
            {
                KeyId = v.LocalizationKeyId,
                CultureCode = v.Culture,
                Value = v.Value,
                Key = v.LocalizationKey.Key
            })
            .OrderBy(v => v.Key)
            .ToListAsync();
    }

    /// <summary>
    /// Inserts or updates a localization value.
    /// </summary>
    /// <param name="dto">The value DTO.</param>
    public async Task UpsertValueAsync(LocalizationValueDto dto)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationValue? existing = await db.LocalizationValues.FirstOrDefaultAsync(v =>
            v.LocalizationKeyId == dto.KeyId &&
            v.Culture == dto.CultureCode);

        if (existing is null)
        {
            db.LocalizationValues.Add(new LocalizationValue
            {
                LocalizationKeyId = dto.KeyId,
                Culture = dto.CultureCode,
                Value = dto.Value
            });
        }
        else
        {
            existing.Value = dto.Value;
        }

        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.ValueUpdated(dto.Key, dto.CultureCode)
        );
    }

    /// <summary>
    /// Exports all localization values as CSV.
    /// </summary>
    /// <param name="search">Optional search filter.</param>
    /// <returns>A UTF‑8 encoded CSV byte array.</returns>
    public async Task<byte[]> ExportCsvAsync(string? search = null)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var q = db.LocalizationValues
            .Select(v => new
            {
                v.LocalizationKey.Key,
                v.Culture,
                v.Value
            });

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Key.Contains(search));

        var rows = await q.OrderBy(x => x.Key).ThenBy(x => x.Culture).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Key,Culture,Value");

        foreach (var r in rows)
        {
            sb.Append(Escape(r.Key)).Append(',')
              .Append(Escape(r.Culture)).Append(',')
              .Append(Escape(r.Value)).AppendLine();
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    /// <summary>
    /// Imports localization values from a CSV stream.
    /// </summary>
    /// <param name="csvStream">The CSV stream.</param>
    public async Task ImportCsvAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream, Encoding.UTF8, leaveOpen: true);

        string? header = await reader.ReadLineAsync();
        if (header is null) return;

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<string> cols = ParseCsvLine(line);
            if (cols.Count < 3) continue;

            string key = cols[0];
            string culture = cols[1];
            string value = cols[2];

            await UpsertByKeyNameAsync(key, culture, value);
        }
    }

    /// <summary>
    /// Exports localization values as JSON.
    /// </summary>
    /// <param name="search">Optional search filter.</param>
    /// <returns>A UTF‑8 encoded JSON byte array.</returns>
    public async Task<byte[]> ExportJsonAsync(string? search = null)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var q = db.LocalizationValues
            .Select(v => new
            {
                key = v.LocalizationKey.Key,
                culture = v.Culture,
                value = v.Value
            });

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.key.Contains(search));

        var rows = await q.ToListAsync();

        return JsonSerializer.SerializeToUtf8Bytes(rows, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Imports localization values from a JSON stream.
    /// </summary>
    /// <param name="jsonStream">The JSON stream.</param>
    public async Task ImportJsonAsync(Stream jsonStream)
    {
        List<JsonRow> items = await JsonSerializer.DeserializeAsync<List<JsonRow>>(jsonStream)
                    ?? new();

        foreach (JsonRow i in items)
            await UpsertByKeyNameAsync(i.key, i.culture, i.value);
    }

    // -------------------------
    // Internal helpers
    // -------------------------

    private async Task UpsertByKeyNameAsync(string key, string culture, string value)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey k = await db.LocalizationKeys.FirstOrDefaultAsync(x => x.Key == key)
            ?? (db.LocalizationKeys.Add(new LocalizationKey { Key = key, IsSystem = true }).Entity);

        await db.SaveChangesAsync();

        LocalizationValue? row = await db.LocalizationValues.FirstOrDefaultAsync(v =>
            v.LocalizationKeyId == k.Id &&
            v.Culture == culture);

        if (row is null)
        {
            db.LocalizationValues.Add(new LocalizationValue
            {
                LocalizationKeyId = k.Id,
                Culture = culture,
                Value = value
            });
        }
        else
        {
            row.Value = value;
        }

        await db.SaveChangesAsync();
    }

    private static string Escape(string s)
        => "\"" + s.Replace("\"", "\"\"") + "\"";

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }

    private sealed record JsonRow(string key, string culture, string value);
}
