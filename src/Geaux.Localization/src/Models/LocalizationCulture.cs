using Geaux.SharedKernal.Entities;

namespace Geaux.Localization.Models;

public sealed class LocalizationCulture : IAuditable, ISoftDelete
{
    public int Id { get; set; }

    /// <summary>
    /// The culture code, e.g. "en-US".
    /// </summary>
    public string CultureCode { get; set; } = default!;

    /// <summary>
    /// Human-friendly display name, e.g. "English (United States)".
    /// </summary>
    public string DisplayName { get; set; } = default!;

    /// <summary>
    /// ISO 3166-1 alpha-2 flag code, e.g. "us".
    /// </summary>
    public string? FlagCode { get; set; }

    /// <summary>
    /// Whether this culture is active and available for selection.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this culture uses right-to-left text direction.
    /// </summary>
    public bool IsRightToLeft { get; set; }

    /// <summary>
    /// Whether this culture requires translation review.
    /// </summary>
    public bool RequiresReview { get; set; }

    /// <summary>
    /// Percentage of keys translated for this culture.
    /// </summary>
    public double PercentTranslated { get; set; }

    /// <summary>
    /// Optional fallback culture code, e.g. "en-US".
    /// </summary>
    public string? FallbackCulture { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
