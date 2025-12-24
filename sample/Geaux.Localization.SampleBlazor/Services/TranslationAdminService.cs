using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Geaux.Localization.SampleBlazor.Services;

/// <summary>
/// Simple admin service for CRUD + bulk import/export operations on translations.
/// </summary>
public sealed class TranslationAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _dbFactory;

    public TranslationAdminService(IDbContextFactory<GeauxLocalizationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Translation>> ListAsync(string? tenantId, string? culture, string? search, CancellationToken ct = default)
    {
        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);

        IQueryable<Translation> q = db.Translations.AsNoTracking();

        tenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim();

        if (tenantId != null)
            q = q.Where(t => t.TenantId == tenantId);
        else
            q = q.Where(t => t.TenantId == null);

        if (!string.IsNullOrWhiteSpace(culture))
            q = q.Where(t => t.Culture == culture.Trim());

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(t => t.Key.Contains(s) || t.Value.Contains(s));
        }

        return await q
            .OrderBy(t => t.Culture)
            .ThenBy(t => t.Key)
            .ToListAsync(ct);
    }

    public async Task<Translation?> GetAsync(int id, CancellationToken ct = default)
    {
        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Translations.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<int> CreateAsync(Translation translation, CancellationToken ct = default)
    {
        Normalize(translation);

        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);
        db.Translations.Add(translation);
        await db.SaveChangesAsync(ct);
        return translation.Id;
    }

    public async Task UpdateAsync(Translation translation, CancellationToken ct = default)
    {
        Normalize(translation);

        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);
        db.Translations.Update(translation);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);
        Translation? entity = await db.Translations.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity == null) return;
        db.Translations.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    // -------- Bulk export --------

    public async Task<byte[]> ExportJsonAsync(string? tenantId, string? culture, string? search, CancellationToken ct = default)
    {
        List<Translation> rows = await ListAsync(tenantId, culture, search, ct);
        var json = JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
        return Encoding.UTF8.GetBytes(json);
    }

    public async Task<byte[]> ExportCsvAsync(string? tenantId, string? culture, string? search, CancellationToken ct = default)
    {
        List<Translation> rows = await ListAsync(tenantId, culture, search, ct);
        var sb = new StringBuilder();
        sb.AppendLine("TenantId,Culture,Key,Value");

        foreach (Translation r in rows)
        {
            sb.Append(EscapeCsv(r.TenantId));
            sb.Append(',');
            sb.Append(EscapeCsv(r.Culture));
            sb.Append(',');
            sb.Append(EscapeCsv(r.Key));
            sb.Append(',');
            sb.Append(EscapeCsv(r.Value));
            sb.AppendLine();
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // -------- Bulk import (upsert) --------

    public async Task<int> ImportJsonAsync(string json, CancellationToken ct = default)
    {
        List<Translation> rows = JsonSerializer.Deserialize<List<Translation>>(json) ?? new List<Translation>();
        return await UpsertManyAsync(rows, ct);
    }

    public async Task<int> ImportCsvAsync(string csv, CancellationToken ct = default)
    {
        var rows = ParsePreviewCsv(csv).ToList();
        return await UpsertManyAsync(rows, ct);
    }

    // -------- Preview parsing (UI) --------

    public IEnumerable<Translation> ParsePreviewJson(string json)
    {
        List<Translation> rows = JsonSerializer.Deserialize<List<Translation>>(json) ?? new List<Translation>();
        foreach (Translation r in rows)
        {
            Normalize(r);
            if (!string.IsNullOrWhiteSpace(r.Culture) && !string.IsNullOrWhiteSpace(r.Key))
                yield return r;
        }
    }

    public IEnumerable<Translation> ParsePreviewCsv(string csv)
    {
        // Normalize line endings, split, keep non-empty
        var lines = csv.Replace("\r\n", "\n").Replace("\r", "\n")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
            yield break;

        // Expect header: TenantId,Culture,Key,Value
        for (int i = 1; i < lines.Length; i++)
        {
            List<string> cols = ParseCsvLine(lines[i]);
            if (cols.Count < 4) continue;

            var t = new Translation
            {
                TenantId = string.IsNullOrWhiteSpace(cols[0]) ? null : cols[0],
                Culture = cols[1],
                Key = cols[2],
                Value = cols[3]
            };

            Normalize(t);

            if (!string.IsNullOrWhiteSpace(t.Culture) && !string.IsNullOrWhiteSpace(t.Key))
                yield return t;
        }
    }

    private async Task<int> UpsertManyAsync(List<Translation> rows, CancellationToken ct)
    {
        if (rows.Count == 0) return 0;

        foreach (Translation r in rows) Normalize(r);

        await using GeauxLocalizationDbContext db = await _dbFactory.CreateDbContextAsync(ct);

        int upserted = 0;

        foreach (Translation r in rows)
        {
            if (string.IsNullOrWhiteSpace(r.Culture) || string.IsNullOrWhiteSpace(r.Key))
                continue;

            Translation? existing = await db.Translations.FirstOrDefaultAsync(t =>
                t.TenantId == r.TenantId &&
                t.Culture == r.Culture &&
                t.Key == r.Key, ct);

            if (existing == null)
            {
                db.Translations.Add(new Translation
                {
                    TenantId = r.TenantId,
                    Culture = r.Culture,
                    Key = r.Key,
                    Value = r.Value ?? ""
                });
            }
            else
            {
                existing.Value = r.Value ?? "";
            }

            upserted++;
        }

        await db.SaveChangesAsync(ct);
        return upserted;
    }

    private static void Normalize(Translation t)
    {
        t.TenantId = string.IsNullOrWhiteSpace(t.TenantId) ? null : t.TenantId.Trim();
        t.Culture = (t.Culture ?? "en-US").Trim();
        t.Key = (t.Key ?? "").Trim();
        t.Value = t.Value ?? "";
    }

    private static string EscapeCsv(string? value)
    {
        value ??= "";
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }
        return value;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // skip escaped quote
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            {
                if (c == ',')
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else
                {
                    sb.Append(c);
                }
            }
        }

        result.Add(sb.ToString());
        return result;
    }
}
