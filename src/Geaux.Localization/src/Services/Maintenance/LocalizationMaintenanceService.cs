namespace Geaux.Localization.Services.Maintenance
{
    public sealed class LocalizationMaintenanceService
    {
        private readonly GeauxLocalizationDbContext _db;

        public LocalizationMaintenanceService(GeauxLocalizationDbContext db)
        {
            _db = db;
        }

        // B: when a new culture is added
        public async Task SeedValuesForCultureAsync(string cultureCode, string? tenantId = null)
        {
            List<LocalizationKey> keys = await _db.LocalizationKeys.AsNoTracking().ToListAsync();

            foreach (LocalizationKey? key in keys)
            {
                bool exists = await _db.LocalizationValues.AnyAsync(v =>
                    v.LocalizationKeyId == key.Id &&
                    v.Culture == cultureCode &&
                    v.TenantId == tenantId);

                if (!exists)
                {
                    _db.LocalizationValues.Add(new LocalizationValue
                    {
                        LocalizationKeyId = key.Id,
                        Culture = cultureCode,
                        TenantId = tenantId,
                        Value = ""
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

        // C: when a new key is added
        public async Task SeedValuesForKeyAsync(int keyId, string? tenantId = null)
        {
            List<LocalizationCulture> cultures = await _db.LocalizationCultures.AsNoTracking().ToListAsync();

            foreach (LocalizationCulture? culture in cultures)
            {
                bool exists = await _db.LocalizationValues.AnyAsync(v =>
                    v.LocalizationKeyId == keyId &&
                    v.Culture == culture.CultureCode &&
                    v.TenantId == tenantId);

                if (!exists)
                {
                    _db.LocalizationValues.Add(new LocalizationValue
                    {
                        LocalizationKeyId = keyId,
                        Culture = culture.CultureCode,
                        TenantId = tenantId,
                        Value = ""
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

        // One-time / periodic repair
        public async Task RepairMissingValuesAsync(string? tenantId = null)
        {
            List<LocalizationKey> keys = await _db.LocalizationKeys.AsNoTracking().ToListAsync();
            List<LocalizationCulture> cultures = await _db.LocalizationCultures.AsNoTracking().ToListAsync();

            foreach (LocalizationKey? key in keys)
            {
                foreach (LocalizationCulture? culture in cultures)
                {
                    bool exists = await _db.LocalizationValues.AnyAsync(v =>
                        v.LocalizationKeyId == key.Id &&
                        v.Culture == culture.CultureCode &&
                        v.TenantId == tenantId);

                    if (!exists)
                    {
                        _db.LocalizationValues.Add(new LocalizationValue
                        {
                            LocalizationKeyId = key.Id,
                            Culture = culture.CultureCode,
                            TenantId = tenantId,
                            Value = ""
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
