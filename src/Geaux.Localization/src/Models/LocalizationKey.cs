using Geaux.SharedKernal.Entities;

namespace Geaux.Localization.Models
{
    /// <summary>
    /// Represents a unique localization key used to group localized values across cultures and tenants.
    /// </summary>
    public sealed class LocalizationKey : IAuditable, ISoftDelete
    {
        /// <summary>
        /// Gets or sets the primary key for the localization key record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the string key that identifies the localized resource.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional description providing context for the key.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key is considered system-owned and protected.
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Gets or sets the collection of localized values associated with this key.
        /// </summary>
        public ICollection<LocalizationValue> Values { get; set; } = new List<LocalizationValue>();

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
}
