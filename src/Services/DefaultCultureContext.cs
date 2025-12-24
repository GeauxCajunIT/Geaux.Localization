using System.Globalization;
using Geaux.Localization.Interfaces;

namespace Geaux.Localization.Services;

/// <summary>
/// Default <see cref="ICultureContext"/> implementation that uses <see cref="CultureInfo.CurrentUICulture"/>.
/// </summary>
public sealed class DefaultCultureContext : ICultureContext
{
    /// <inheritdoc />
    public string CurrentCulture => CultureInfo.CurrentUICulture.Name;
}
