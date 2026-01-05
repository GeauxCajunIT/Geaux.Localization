namespace Geaux.Shared.Activity;

/// <summary>
/// Centralized factory for creating ActivityLog entries.
/// Ensures consistent formatting, timestamps, and event typing
/// across all admin services.
/// </summary>
public static class ActivityLogFactory
{
    // ------------------------------------------------------------
    // KEYS
    // ------------------------------------------------------------

    public static ActivityLog KeyCreated(string key, string? metadata = null)
        => Create(ActivityType.KeyCreated, $"Created key: {key}", metadata);

    public static ActivityLog KeyUpdated(string key, string? metadata = null)
        => Create(ActivityType.KeyUpdated, $"Updated key: {key}", metadata);

    public static ActivityLog KeyDeleted(string key, string? metadata = null)
        => Create(ActivityType.KeyDeleted, $"Deleted key: {key}", metadata);

    // ------------------------------------------------------------
    // VALUES
    // ------------------------------------------------------------

    public static ActivityLog ValueUpdated(string key, string culture, string? metadata = null)
        => Create(ActivityType.TranslationUpdated,
            $"Updated value for '{key}' in {culture}",
            metadata);

    public static ActivityLog MissingTranslationsRepaired(int count, string? metadata = null)
        => Create(ActivityType.MissingTranslationsRepaired,
            $"Repaired {count} missing translations",
            metadata);

    // ------------------------------------------------------------
    // CULTURES
    // ------------------------------------------------------------

    public static ActivityLog CultureCreated(string culture, string? metadata = null)
        => Create(ActivityType.CultureAdded, $"Added culture: {culture}", metadata);

    public static ActivityLog CultureUpdated(string culture, string? metadata = null)
        => Create(ActivityType.CultureUpdated, $"Updated culture: {culture}", metadata);

    public static ActivityLog CultureDeleted(string culture, string? metadata = null)
        => Create(ActivityType.CultureDeleted, $"Deleted culture: {culture}", metadata);

    public static ActivityLog CultureActivated(string culture, string? metadata = null)
        => Create(ActivityType.CultureActivated, $"Activated culture: {culture}", metadata);

    public static ActivityLog CultureDeactivated(string culture, string? metadata = null)
        => Create(ActivityType.CultureDeactivated, $"Deactivated culture: {culture}", metadata);

    public static ActivityLog CultureMarkedForReview(string culture, string? metadata = null)
        => Create(ActivityType.CultureMarkedForReview, $"Marked culture for review: {culture}", metadata);

    public static ActivityLog CultureReviewCleared(string culture, string? metadata = null)
        => Create(ActivityType.CultureReviewCleared, $"Cleared review flag for culture: {culture}", metadata);

    // ------------------------------------------------------------
    // LANGUAGE PACKS
    // ------------------------------------------------------------

    public static ActivityLog LanguagePackExported(string culture, string? metadata = null)
        => Create(ActivityType.LanguagePackExported,
            $"Exported language pack for {culture}",
            metadata);

    public static ActivityLog LanguagePackImported(string culture, string? metadata = null)
        => Create(ActivityType.LanguagePackImported,
            $"Imported language pack for {culture}",
            metadata);

    // ------------------------------------------------------------
    // INTERNAL CREATOR
    // ------------------------------------------------------------

    private static ActivityLog Create(string type, string description, string? metadata)
        => new ActivityLog(
            activityType: type,
            description: description,
            tenantId: null,
            metadataJson: metadata,
            timestampUtc: DateTime.UtcNow
        );
}
