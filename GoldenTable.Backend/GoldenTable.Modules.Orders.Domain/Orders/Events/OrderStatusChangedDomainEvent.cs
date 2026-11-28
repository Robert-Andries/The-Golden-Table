using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Domain.Orders.Events;

public sealed class OrderStatusChangedDomainEvent(Guid orderId, OrderStatus newStatus) : DomainEvent
{
    public Guid OrderId { get; } = orderId;
    public OrderStatus NewStatus { get; } = newStatus;
}
