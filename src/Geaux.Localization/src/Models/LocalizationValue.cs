using Geaux.SharedKernal.Entities;

namespace Geaux.Localization.Models;

/// <summary>
/// Represents a localized value associated with a specific key, culture, and tenant scope.
/// </summary>
public sealed class LocalizationValue : IAuditable, ISoftDelete
{
    /// <summary>
    /// Gets or sets the primary key for the localized value.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated <see cref="LocalizationKey"/>.
    /// </summary>
    public int LocalizationKeyId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent <see cref="LocalizationKey"/>.
    /// </summary>
    public LocalizationKey LocalizationKey { get; set; } = default!;

    /// <summary>
    /// Gets or sets the culture identifier (for example, <c>en-US</c>) for the value.
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the tenant identifier that scopes this value. Null represents the global tenant.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the localized text.
    /// </summary>
    public string Value { get; set; } = string.Empty;

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
