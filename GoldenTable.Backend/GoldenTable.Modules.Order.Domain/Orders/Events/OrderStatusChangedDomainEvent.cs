using GoldenTable.Common.Domain;
using GoldenTable.Modules.Order.Domain.Orders.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace GoldenTable.Modules.Order.Domain.Orders.Events;

public sealed class OrderStatusChangedDomainEvent(Guid orderId, OrderStatus newStatus) : DomainEvent
{
    public Guid OrderId { get; } = orderId;
    public OrderStatus NewStatus { get; } = newStatus;
}
