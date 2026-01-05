namespace Geaux.Localization.Services.ExportImport
{
    using System.IO.Compression;
    using System.Text;
    using System.Text.Json;

    public sealed class LocalizationExportService
    {
        private readonly GeauxLocalizationDbContext _db;

        public LocalizationExportService(GeauxLocalizationDbContext db)
        {
            _db = db;
        }

        // ------------------------------------------------------------
        // CULTURE LIST
        // ------------------------------------------------------------
        public async Task<IReadOnlyList<LocalizationCulture>> GetAllCulturesAsync()
            => await _db.LocalizationCultures.AsNoTracking().ToListAsync();

        // ------------------------------------------------------------
        // LOAD KEYS + VALUES FOR A CULTURE
        // ------------------------------------------------------------
        public async Task<(List<LocalizationKey> Keys, List<LocalizationValue> Values)>
            GetKeysAndValuesForCultureAsync(string culture, string? tenantId = null)
        {
            List<LocalizationKey> keys = await _db.LocalizationKeys
                .AsNoTracking()
                .ToListAsync();

            IQueryable<LocalizationValue> valuesQuery = _db.LocalizationValues
                .AsNoTracking()
                .Where(v => v.Culture == culture);

            if (tenantId is not null)
                valuesQuery = valuesQuery.Where(v => v.TenantId == tenantId);

            List<LocalizationValue> values = await valuesQuery.ToListAsync();

            return (keys, values);
        }

        // ------------------------------------------------------------
        // JSON EXPORT (FULL)
        // ------------------------------------------------------------
        public Dictionary<string, string> GenerateJsonForCulture(
            string culture,
            List<LocalizationKey> keys,
            List<LocalizationValue> values)
        {
            return keys.ToDictionary(
                k => k.Key,
                k => values.FirstOrDefault(v => v.LocalizationKeyId == k.Id)?.Value ?? ""
            );
        }

        // ------------------------------------------------------------
        // JSON EXPORT (MISSING ONLY)
        // ------------------------------------------------------------
        public Dictionary<string, string> GenerateMissingJsonForCulture(
            string culture,
            List<LocalizationKey> keys,
            List<LocalizationValue> values)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (LocalizationKey key in keys)
            {
                string? value = values.FirstOrDefault(v => v.LocalizationKeyId == key.Id)?.Value;

                if (string.IsNullOrWhiteSpace(value))
                    dict[key.Key] = "";
            }

            return dict;
        }

        // ------------------------------------------------------------
        // CSV EXPORT (FULL)
        // Format A: Key, Culture, Value
        // ------------------------------------------------------------
        public string GenerateCsvForCulture(
            string culture,
            List<LocalizationKey> keys,
            List<LocalizationValue> values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Key,Culture,Value");

            foreach (LocalizationKey key in keys)
            {
                string value = values.FirstOrDefault(v => v.LocalizationKeyId == key.Id)?.Value ?? "";
                sb.AppendLine($"{Escape(key.Key)},{Escape(culture)},{Escape(value)}");
            }

            return sb.ToString();
        }

        // ------------------------------------------------------------
        // CSV EXPORT (MISSING ONLY)
        // ------------------------------------------------------------
        public string GenerateMissingCsvForCulture(
            string culture,
            List<LocalizationKey> keys,
            List<LocalizationValue> values)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Key,Culture,Value");

            foreach (LocalizationKey key in keys)
            {
                string? value = values.FirstOrDefault(v => v.LocalizationKeyId == key.Id)?.Value;

                if (string.IsNullOrWhiteSpace(value))
                    sb.AppendLine($"{Escape(key.Key)},{Escape(culture)},");
            }

            return sb.ToString();
        }

        // ------------------------------------------------------------
        // ZIP EXPORT (ALL CULTURES)
        // ------------------------------------------------------------
        public async Task<byte[]> GenerateZipForAllCulturesAsync(string? tenantId = null)
        {
            IReadOnlyList<LocalizationCulture> cultures = await GetAllCulturesAsync();

            using MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (LocalizationCulture culture in cultures)
                {
                    (List<LocalizationKey>? keys, List<LocalizationValue>? values) = await GetKeysAndValuesForCultureAsync(culture.CultureCode, tenantId);

                    string json = JsonSerializer.Serialize(
                        GenerateJsonForCulture(culture.CultureCode, keys, values),
                        new JsonSerializerOptions { WriteIndented = true });

                    ZipArchiveEntry entry = zip.CreateEntry($"{culture.CultureCode}.json");

                    using Stream entryStream = entry.Open();
                    using StreamWriter writer = new StreamWriter(entryStream, Encoding.UTF8);
                    writer.Write(json);
                }
            }

            return ms.ToArray();
        }

        // ------------------------------------------------------------
        // CSV ESCAPE
        // ------------------------------------------------------------
        private static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return $"\"{input.Replace("\"", "\"\"")}\"";
        }
    }
}