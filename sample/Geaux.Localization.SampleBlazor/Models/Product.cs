using Geaux.Localization.Attributes;

namespace Geaux.Localization.SampleBlazor.Models;

/// <summary>
/// Demo model showing how to use <see cref="LocalizedAttribute"/> for display names and messages.
/// </summary>
public sealed class Product
{
    [Localized("Product.Name", DisplayNameKey = "Product.Name.Display")]
    public string Name { get; set; } = string.Empty;

    [Localized("Product.Sku", DisplayNameKey = "Product.Sku.Display")]
    public string Sku { get; set; } = string.Empty;

    [Localized("Product.Price", DisplayNameKey = "Product.Price.Display")]
    public decimal Price { get; set; }

    [Localized("Product.Description", DisplayNameKey = "Product.Description.Display")]
    public string? Description { get; set; }
}
