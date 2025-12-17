namespace Geaux.Localization.Attributes;

/// <summary>
/// Specifies that a property should be associated with a localized resource key for display, validation,
/// or messaging purposes.
/// </summary>
/// <remarks>
/// Apply this attribute to a property to enable localization of its display name, error messages, or other
/// user-facing text. The attribute allows specifying resource keys for different localization scenarios and
/// supports culture-specific overrides. Only one instance of this attribute can be applied to a property.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocalizedAttribute : Attribute
{
    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the culture identifier associated with the current context.
    /// </summary>
    /// <remarks>
    /// The culture identifier is typically a language and region code, such as "en-US" or "fr-FR".
    /// This value may be null if no culture is specified.
    /// </remarks>
    public string? Culture { get; }

    /// <summary>
    /// Gets a value indicating whether the operation should fall back to a default value when a specific
    /// value is unavailable.
    /// </summary>
    public bool FallbackToDefault { get; }

    /// <summary>
    /// Gets the resource key that identifies the error message associated with the current result.
    /// </summary>
    public string? ErrorMessageKey { get; }

    /// <summary>
    /// Gets the resource key used to look up the display name for this item.
    /// </summary>
    public string? DisplayNameKey { get; }

    /// <summary>
    /// Gets the resource key used to retrieve the display message associated with this instance.
    /// </summary>
    public string? DisplayMessageKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedAttribute"/> class with the specified localization
    /// keys and options.
    /// </summary>
    /// <param name="key">The key that identifies the localized resource. Cannot be null or empty.</param>
    /// <param name="culture">The culture code (such as "en-US") to use for localization. If null, the current culture is used.</param>
    /// <param name="fallbackToDefault">True to fall back to the default culture if the specified culture is not found; otherwise, false.</param>
    /// <param name="errorMessageKey">The key for the localized error message. If null, no error message is associated.</param>
    /// <param name="displayNameKey">The key for the localized display name. If null, no display name is associated.</param>
    /// <param name="displayMessageKey">The key for the localized display message. If null, no display message is associated.</param>
    public LocalizedAttribute(
        string key,
        string? culture = null,
        bool fallbackToDefault = true,
        string? errorMessageKey = null,
        string? displayNameKey = null,
        string? displayMessageKey = null)
    {
        Key = key;
        Culture = culture;
        FallbackToDefault = fallbackToDefault;
        ErrorMessageKey = errorMessageKey;
        DisplayNameKey = displayNameKey;
        DisplayMessageKey = displayMessageKey;
    }
}
