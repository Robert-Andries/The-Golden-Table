using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Presentation.Orders.Common;

/// <summary>
///     What the API returns for an order. Keeps the domain entity (and its domain events) out of responses.
/// </summary>
internal sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    bool CancelRequested,
    decimal TotalPriceAmount,
    string TotalPriceCurrency,
    IReadOnlyCollection<OrderItemResponse> Items,
    AddressResponse Address,
    DateTime CreatedOnUtc,
    DateTime ModifiedOnUtc)
{
    internal static OrderResponse FromOrder(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CancelRequested,
            order.TotalPrice.Amount,
            order.TotalPrice.Currency.Code,
            order.Items.Select(i => new OrderItemResponse(i.DishId, i.SelectedSizeName, i.Quantity)).ToList(),
            new AddressResponse(order.Address.County, order.Address.City, order.Address.Street,
                order.Address.StreetNumber, order.Address.ZipCode),
            order.CreatedOnUtc,
            order.ModifiedOnUtc);
    }

    internal static IReadOnlyCollection<OrderResponse> FromOrders(IEnumerable<Order> orders)
    {
        return orders.Select(FromOrder).ToList();
    }
}

internal sealed record OrderItemResponse(Guid DishId, string? SizeName, int Quantity);

internal sealed record AddressResponse(string County, string City, string Street, string StreetNumber, string ZipCode);
