using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Order.Domain.Orders.Events;

public sealed class OrderCancelRequestedDomainEvent(Guid orderId) : DomainEvent
{
    public Guid OrderId { get; } =  orderId;
}
