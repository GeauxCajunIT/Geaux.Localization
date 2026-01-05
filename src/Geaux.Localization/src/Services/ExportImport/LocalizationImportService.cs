namespace Geaux.Localization.Services.ExportImport
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using System.IO.Compression;
    using System.Text.Json;

    public sealed class LocalizationImportService
    {
        private readonly GeauxLocalizationDbContext _db;

        public LocalizationImportService(GeauxLocalizationDbContext db)
        {
            _db = db;
        }

        public async Task<ImportResult> ImportJsonAsync(
            string culture,
            Stream jsonStream,
            string? tenantId = null)
        {
            using StreamReader reader = new StreamReader(jsonStream);
            string json = await reader.ReadToEndAsync();

            Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                       ?? new Dictionary<string, string>();

            return await UpsertValuesAsync(culture, dict, tenantId);
        }

        public async Task<ImportResult> ImportCsvAsync(
            string culture,
            Stream csvStream,
            string? tenantId = null)
        {
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true
            };

            using StreamReader reader = new StreamReader(csvStream);
            using CsvReader csv = new CsvReader(reader, config);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            while (await csv.ReadAsync())
            {
                string? key = csv.GetField("Key");
                string? value = csv.GetField("Value");

                if (!string.IsNullOrWhiteSpace(key))
                    dict[key] = value ?? string.Empty;
            }

            return await UpsertValuesAsync(culture, dict, tenantId);
        }

        public async Task<ImportResult> ImportZipAsync(
            Stream zipStream,
            string? tenantId = null)
        {
            ImportResult result = new ImportResult();

            using ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                if (!entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                string culture = Path.GetFileNameWithoutExtension(entry.FullName);

                using Stream entryStream = entry.Open();
                ImportResult r = await ImportJsonAsync(culture, entryStream, tenantId);

                result.Merge(r);
            }

            return result;
        }

        private async Task<ImportResult> UpsertValuesAsync(
            string culture,
            Dictionary<string, string> dict,
            string? tenantId)
        {
            ImportResult result = new ImportResult();

            List<LocalizationKey> keys = await _db.LocalizationKeys.ToListAsync();

            foreach ((string? keyName, string? value) in dict)
            {
                LocalizationKey? key = keys.FirstOrDefault(k => k.Key == keyName);

                if (key is null)
                {
                    result.MissingKeys.Add(keyName);
                    continue;
                }

                LocalizationValue? existing = await _db.LocalizationValues
                    .FirstOrDefaultAsync(v =>
                        v.LocalizationKeyId == key.Id &&
                        v.Culture == culture &&
                        v.TenantId == tenantId);

                if (existing is null)
                {
                    _db.LocalizationValues.Add(new LocalizationValue
                    {
                        LocalizationKeyId = key.Id,
                        Culture = culture,
                        TenantId = tenantId,
                        Value = value ?? string.Empty
                    });

                    result.Inserted++;
                }
                else
                {
                    existing.Value = value ?? string.Empty;
                    result.Updated++;
                }
            }

            await _db.SaveChangesAsync();
            return result;
        }
    }

    public sealed class ImportResult
    {
        public int Inserted { get; set; }
        public int Updated { get; set; }
        public List<string> MissingKeys { get; set; } = new();

        public void Merge(ImportResult other)
        {
            Inserted += other.Inserted;
            Updated += other.Updated;
            MissingKeys.AddRange(other.MissingKeys);
        }
    }
}
