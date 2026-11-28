using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Domain.Orders.Events;

public sealed class OrderItemsChangedDomainEvent(Guid orderId, IReadOnlyCollection<DishItem> newItems) : DomainEvent
{
    public Guid OrderId { get; } = orderId;
    public IReadOnlyCollection<DishItem> NewItems { get; } = newItems;
}
