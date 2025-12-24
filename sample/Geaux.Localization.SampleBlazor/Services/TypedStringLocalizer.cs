using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Geaux.Localization.SampleBlazor.Services;

/// <summary>
/// Typed localizer adapter that resolves a type-scoped <see cref="IStringLocalizer"/> using the registered <see cref="IStringLocalizerFactory"/>.
/// </summary>
/// <typeparam name="T">The resource type.</typeparam>
public sealed class TypedStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizer _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedStringLocalizer{T}"/> class.
    /// </summary>
    /// <param name="factory">The localizer factory.</param>
    public TypedStringLocalizer(IStringLocalizerFactory factory)
    {
        _inner = factory.Create(typeof(T));
    }

    /// <inheritdoc />
    public LocalizedString this[string name] => _inner[name];

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] => _inner[name, arguments];

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _inner.GetAllStrings(includeParentCultures);

    /// <summary>
    /// Returns a localizer for the specified culture when supported by the underlying implementation.
    /// </summary>
    /// <remarks>
    /// Some versions of <c>Microsoft.Extensions.Localization</c> do not expose <c>WithCulture</c> on the interface.
    /// Geaux's database localizer supports culture scoping; this adapter uses reflection to call it when available.
    /// </remarks>
    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        var method = _inner.GetType().GetMethod("WithCulture", new[] { typeof(CultureInfo) });
        if (method is null)
        {
            return _inner;
        }

        var result = method.Invoke(_inner, new object[] { culture });
        return result as IStringLocalizer ?? _inner;
    }
}
