namespace GoldenTable.Modules.Orders.Presentation.Orders.Common;

/// <summary>
///     One line of an order, as the client sends it
/// </summary>
internal sealed record OrderItemRequest(Guid DishId, string? SizeName, int Quantity)
{
    internal static IReadOnlyCollection<(Guid id, string? itemSizeName, int quantity)> ToCommandItems(
        IReadOnlyCollection<OrderItemRequest>? items)
    {
        return items?.Select(i => (i.DishId, i.SizeName, i.Quantity)).ToList() ?? [];
    }
}
