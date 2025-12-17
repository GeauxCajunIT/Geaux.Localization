// // <copyright file="" company="GeauxCajunIT">
// // Copyright (c) GeauxCajunIT. All rights reserved.
// // </copyright>

using Geaux.Localization.Attributes;
using Microsoft.Extensions.Localization;

/// <summary>
/// Provides methods for applying localized string values to model properties marked with localization attributes.
/// </summary>
/// <remarks>This class is intended to assist with populating model properties with localized values based on the
/// specified culture and tenant context. It operates on properties decorated with a specific localization attribute and
/// uses an IStringLocalizer to resolve localized strings. The class is static and cannot be instantiated.</remarks>
public static class LocalizationResolver
{
    /// <summary>
    /// Applies localized string values to properties of the specified model that are marked with the
    /// LocalizedAttribute.
    /// </summary>
    /// <remarks>Only properties decorated with the LocalizedAttribute are affected. If a localized resource
    /// is not found and the attribute's FallbackToDefault property is set, the default value will be used. This method
    /// updates the model in place.</remarks>
    /// <typeparam name="T">The type of the model whose properties will be localized. Must be a reference type.</typeparam>
    /// <param name="model">The model instance whose properties will be updated with localized values. Cannot be null.</param>
    /// <param name="localizer">The string localizer used to retrieve localized values. Cannot be null.</param>
    /// <param name="tenantId">The identifier of the tenant for which localization is being applied. Used to determine the appropriate
    /// localization context.</param>
    /// <param name="culture">The culture code (such as "en-US") specifying which localization to apply. Cannot be null or empty.</param>
    public static void ApplyLocalization<T>(
        T model,
        IStringLocalizer localizer,
        string tenantId,
        string culture)
    {
        var props = typeof(T).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(LocalizedAttribute)));

        foreach (var prop in props)
        {
            var attr = (LocalizedAttribute)Attribute.GetCustomAttribute(prop, typeof(LocalizedAttribute))!;
            var key = attr.Key;
            var localized = localizer[key];

            if (!localized.ResourceNotFound || attr.FallbackToDefault)
            {
                prop.SetValue(model, localized.Value);
            }
        }
    }
}

