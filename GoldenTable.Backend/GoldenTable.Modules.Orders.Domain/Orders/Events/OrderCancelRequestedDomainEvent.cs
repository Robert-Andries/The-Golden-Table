using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Orders.Events;

public sealed class OrderCancelRequestedDomainEvent(Guid orderId) : DomainEvent
{
    public Guid OrderId { get; } = orderId;
}
