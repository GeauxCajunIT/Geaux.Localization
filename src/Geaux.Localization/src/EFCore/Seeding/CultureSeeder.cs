namespace Geaux.Localization.EFCore.Seeding;

public sealed class CultureSeeder
{
    private readonly GeauxLocalizationDbContext _db;

    public CultureSeeder(GeauxLocalizationDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {

        if (await _db.LocalizationCultures.AnyAsync(ct))
            return; // Already seeded

        List<LocalizationCulture> cultures = GetCultures();

        _db.LocalizationCultures.AddRange(cultures);
        await _db.SaveChangesAsync(ct);
    }

    private static List<LocalizationCulture> GetCultures()
    {
        return new()
        {
            // English
            new LocalizationCulture { CultureCode = "en-US", DisplayName = "English (United States)", FlagCode = "us", IsActive = true },
            new LocalizationCulture { CultureCode = "en-GB", DisplayName = "English (United Kingdom)", FlagCode = "gb", FallbackCulture = "en-US" },
            new LocalizationCulture { CultureCode = "en-CA", DisplayName = "English (Canada)", FlagCode = "ca", FallbackCulture = "en-US" },
            new LocalizationCulture { CultureCode = "en-AU", DisplayName = "English (Australia)", FlagCode = "au", FallbackCulture = "en-US" },

            // French
            new LocalizationCulture { CultureCode = "fr-FR", DisplayName = "Français (France)", FlagCode = "fr" },
            new LocalizationCulture { CultureCode = "fr-CA", DisplayName = "Français (Canada)", FlagCode = "ca", FallbackCulture = "fr-FR" },

            // Spanish
            new LocalizationCulture {  CultureCode = "es-ES", DisplayName = "Español (España)", FlagCode = "es" },
            new LocalizationCulture { CultureCode = "es-MX", DisplayName = "Español (México)", FlagCode = "mx", FallbackCulture = "es-ES" },

            // German
            new LocalizationCulture { CultureCode = "de-DE", DisplayName = "Deutsch (Deutschland)", FlagCode = "de" },

            // Italian
            new LocalizationCulture {  CultureCode = "it-IT", DisplayName = "Italiano (Italia)", FlagCode = "it" },

            // Portuguese
            new LocalizationCulture { CultureCode = "pt-PT", DisplayName = "Português (Portugal)", FlagCode = "pt" },
            new LocalizationCulture { CultureCode = "pt-BR", DisplayName = "Português (Brasil)", FlagCode = "br", FallbackCulture = "pt-PT" },

            // Nordic / Dutch
            new LocalizationCulture { CultureCode = "nl-NL", DisplayName = "Nederlands (Nederland)", FlagCode = "nl" },
            new LocalizationCulture { CultureCode = "sv-SE", DisplayName = "Svenska (Sverige)", FlagCode = "se" },
            new LocalizationCulture { CultureCode = "no-NO", DisplayName = "Norsk (Norge)", FlagCode = "no" },
            new LocalizationCulture { CultureCode = "da-DK", DisplayName = "Dansk (Danmark)", FlagCode = "dk" },
            new LocalizationCulture { CultureCode = "fi-FI", DisplayName = "Suomi (Suomi)", FlagCode = "fi" },

            // Eastern Europe
            new LocalizationCulture { CultureCode = "pl-PL", DisplayName = "Polski (Polska)", FlagCode = "pl" },
            new LocalizationCulture { CultureCode = "cs-CZ", DisplayName = "Čeština (Česko)", FlagCode = "cz" },
            new LocalizationCulture { CultureCode = "sk-SK", DisplayName = "Slovenčina (Slovensko)", FlagCode = "sk" },
            new LocalizationCulture { CultureCode = "sl-SI", DisplayName = "Slovenščina (Slovenija)", FlagCode = "si" },
            new LocalizationCulture { CultureCode = "hu-HU", DisplayName = "Magyar (Magyarország)", FlagCode = "hu" },
            new LocalizationCulture { CultureCode = "ro-RO", DisplayName = "Română (România)", FlagCode = "ro" },
            new LocalizationCulture { CultureCode = "bg-BG", DisplayName = "Български (България)", FlagCode = "bg" },

            // Cyrillic
            new LocalizationCulture { CultureCode = "ru-RU", DisplayName = "Русский (Россия)", FlagCode = "ru" },
            new LocalizationCulture { CultureCode = "uk-UA", DisplayName = "Українська (Україна)", FlagCode = "ua" },

            // Middle East (RTL)
            new LocalizationCulture { CultureCode = "ar-SA", DisplayName = "العربية (السعودية)", FlagCode = "sa", IsRightToLeft = true },
            new LocalizationCulture { CultureCode = "ar-AE", DisplayName = "العربية (الإمارات)", FlagCode = "ae", IsRightToLeft = true },
            new LocalizationCulture { CultureCode = "he-IL", DisplayName = "עברית (ישראל)", FlagCode = "il", IsRightToLeft = true },

            // Asia
            new LocalizationCulture { CultureCode = "hi-IN", DisplayName = "हिन्दी (भारत)", FlagCode = "in" },
            new LocalizationCulture { CultureCode = "bn-BD", DisplayName = "বাংলা (বাংলাদেশ)", FlagCode = "bd" },
            new LocalizationCulture { CultureCode = "ta-IN", DisplayName = "தமிழ் (இந்தியா)", FlagCode = "in" },
            new LocalizationCulture { CultureCode = "th-TH", DisplayName = "ไทย (ไทย)", FlagCode = "th" },
            new LocalizationCulture { CultureCode = "vi-VN", DisplayName = "Tiếng Việt (Việt Nam)", FlagCode = "vn" },

            // CJK
            new LocalizationCulture { CultureCode = "zh-CN", DisplayName = "中文 (中国)", FlagCode = "cn" },
            new LocalizationCulture { CultureCode = "zh-TW", DisplayName = "中文 (台灣)", FlagCode = "tw", FallbackCulture = "zh-CN" },
            new LocalizationCulture { CultureCode = "ja-JP", DisplayName = "日本語 (日本)", FlagCode = "jp" },
            new LocalizationCulture { CultureCode = "ko-KR", DisplayName = "한국어 (대한민국)", FlagCode = "kr" }
        };
    }
}
