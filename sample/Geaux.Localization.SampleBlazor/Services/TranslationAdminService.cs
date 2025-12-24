using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Geaux.Localization.SampleBlazor.Services;

public sealed class TranslationAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;

    public TranslationAdminService(IDbContextFactory<GeauxLocalizationDbContext> factory)
        => _factory = factory;

    public async Task<List<LocalizationKey>> SearchKeysAsync(string? search, int take = 200)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        IQueryable<LocalizationKey> q = db.LocalizationKeys.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(k => k.Key.Contains(search));

        return await q.OrderBy(k => k.Key).Take(take).ToListAsync();
    }

    public async Task<LocalizationKey> GetOrCreateKeyAsync(string key, bool isSystem = true)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey? existing = await db.LocalizationKeys.FirstOrDefaultAsync(k => k.Key == key);
        if (existing != null) return existing;

        LocalizationKey created = new LocalizationKey { Key = key, IsSystem = isSystem };
        db.LocalizationKeys.Add(created);
        await db.SaveChangesAsync();
        return created;
    }

    public async Task<string?> GetValueAsync(string key, string culture, string? tenantId)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        return await db.LocalizationValues.AsNoTracking()
            .Where(v => v.LocalizationKey.Key == key &&
                        v.Culture == culture &&
                        v.TenantId == tenantId)
            .Select(v => v.Value)
            .FirstOrDefaultAsync();
    }

    public async Task UpsertValueAsync(string key, string culture, string? tenantId, string value)
    {
        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId;

        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey k = await db.LocalizationKeys.FirstOrDefaultAsync(x => x.Key == key)
            ?? (db.LocalizationKeys.Add(new LocalizationKey { Key = key, IsSystem = true }).Entity);

        await db.SaveChangesAsync(); // ensure key has Id

        LocalizationValue? row = await db.LocalizationValues.FirstOrDefaultAsync(v =>
            v.LocalizationKeyId == k.Id &&
            v.Culture == culture &&
            v.TenantId == tenantId);

        if (row == null)
        {
            db.LocalizationValues.Add(new LocalizationValue
            {
                LocalizationKeyId = k.Id,
                Culture = culture,
                TenantId = tenantId,
                Value = value
            });
        }
        else
        {
            row.Value = value;
        }

        await db.SaveChangesAsync();
    }

    // --- Bulk CSV ---
    // CSV format: Key,Culture,TenantId,Value
    public async Task<byte[]> ExportCsvAsync(string? search = null)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var q = db.LocalizationValues.AsNoTracking()
            .Select(v => new { v.LocalizationKey.Key, v.Culture, v.TenantId, v.Value });

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Key.Contains(search));

        var rows = await q.OrderBy(x => x.Key).ThenBy(x => x.Culture).ToListAsync();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Key,Culture,TenantId,Value");

        foreach (var r in rows)
        {
            sb.Append(Escape(r.Key)).Append(',')
              .Append(Escape(r.Culture)).Append(',')
              .Append(Escape(r.TenantId ?? "")).Append(',')
              .Append(Escape(r.Value)).AppendLine();
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task ImportCsvAsync(Stream csvStream)
    {
        using StreamReader reader = new StreamReader(csvStream, Encoding.UTF8, leaveOpen: true);
        var header = await reader.ReadLineAsync(); // Key,Culture,TenantId,Value
        if (header == null) return;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<string> cols = ParseCsvLine(line);
            if (cols.Count < 4) continue;

            var key = cols[0];
            var culture = cols[1];
            var tenant = string.IsNullOrWhiteSpace(cols[2]) ? null : cols[2];
            var value = cols[3];

            await UpsertValueAsync(key, culture, tenant, value);
        }
    }

    // --- Bulk JSON ---
    // JSON array: [{ "key":"X", "culture":"en-US", "tenantId":null, "value":"..." }]
    public async Task<byte[]> ExportJsonAsync(string? search = null)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var q = db.LocalizationValues.AsNoTracking()
            .Select(v => new { key = v.LocalizationKey.Key, culture = v.Culture, tenantId = v.TenantId, value = v.Value });

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.key.Contains(search));

        var rows = await q.ToListAsync();
        return JsonSerializer.SerializeToUtf8Bytes(rows, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task ImportJsonAsync(Stream jsonStream)
    {
        List<JsonRow> items = await JsonSerializer.DeserializeAsync<List<JsonRow>>(jsonStream)
                    ?? new();

        foreach (JsonRow i in items)
            await UpsertValueAsync(i.key, i.culture, i.tenantId, i.value);
    }

    private sealed record JsonRow(string key, string culture, string? tenantId, string value);

    private static string Escape(string s)
        => "\"" + s.Replace("\"", "\"\"") + "\"";

    private static List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        StringBuilder sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

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

    public sealed record ImportPreviewRow(string Key, string Culture, string? TenantId, string Value);

    public List<ImportPreviewRow> ParsePreviewCsv(string csvText, int maxRows = 50)
    {
        List<ImportPreviewRow> rows = new List<ImportPreviewRow>();
        using StringReader reader = new StringReader(csvText);

        // Header
        var header = reader.ReadLine();
        if (header == null) return rows;

        string? line;
        while ((line = reader.ReadLine()) != null && rows.Count < maxRows)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = ParseCsvLine(line);
            if (cols.Count < 4) continue;

            rows.Add(new ImportPreviewRow(
                cols[0],
                cols[1],
                string.IsNullOrWhiteSpace(cols[2]) ? null : cols[2],
                cols[3]
            ));
        }

        return rows;
    }

    public List<ImportPreviewRow> ParsePreviewJson(string jsonText, int maxRows = 50)
    {
        var rows = System.Text.Json.JsonSerializer.Deserialize<List<ImportPreviewRowJson>>(jsonText)
                   ?? new();

        return rows.Take(maxRows)
                   .Select(x => new ImportPreviewRow(x.key, x.culture, x.tenantId, x.value))
                   .ToList();
    }

    private sealed record ImportPreviewRowJson(string key, string culture, string? tenantId, string value);

}
