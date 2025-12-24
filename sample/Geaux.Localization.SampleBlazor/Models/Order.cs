using Geaux.Localization.Attributes;

namespace Geaux.Localization.SampleBlazor.Models;

/// <summary>
/// Demo model showing how to use <see cref="LocalizedAttribute"/>.
/// </summary>
public sealed class Order
{
    [Localized("Order.Number", DisplayNameKey = "Order.Number.Display")]
    public string Number { get; set; } = string.Empty;

    [Localized("Order.Customer", DisplayNameKey = "Order.Customer.Display")]
    public string CustomerName { get; set; } = string.Empty;

    [Localized("Order.Total", DisplayNameKey = "Order.Total.Display")]
    public decimal Total { get; set; }

    [Localized("Order.Status",
        DisplayNameKey = "Order.Status.Display",
        DisplayMessageKey = "Order.Status.Message",
        ErrorMessageKey = "Order.Status.Error"
        )]
    public string Status { get; set; } = "Pending";
}
