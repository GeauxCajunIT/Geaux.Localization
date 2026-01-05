using Geaux.GuardClauses;

namespace Geaux.Shared.Activity;

public sealed class ActivityLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Optional tenant identifier for multi-tenant systems.
    /// </summary>
    public string? TenantId { get; private set; }

    /// <summary>
    /// The type of activity (CreatedKey, UpdatedTranslation, ActivatedCulture, etc.)
    /// </summary>
    public string ActivityType { get; private set; }

    /// <summary>
    /// A localized, human-readable description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Optional metadata stored as JSON (key/value pairs).
    /// </summary>
    public string? MetadataJson { get; private set; }

    /// <summary>
    /// When the activity occurred (UTC).
    /// </summary>
    public DateTime? TimestampUtc { get; private set; } = DateTime.UtcNow;

    private ActivityLog() { } // EF Core

    public ActivityLog(
        string activityType,
        string description,
        string? tenantId = null,
        string? metadataJson = null,
        DateTime? timestampUtc = null)
    {
        ActivityType = Guard.Against.NullOrWhiteSpace(activityType);
        Description = Guard.Against.NullOrWhiteSpace(description);
        TenantId = tenantId;
        MetadataJson = metadataJson;
        TimestampUtc = timestampUtc ?? DateTime.UtcNow;
    }
}

