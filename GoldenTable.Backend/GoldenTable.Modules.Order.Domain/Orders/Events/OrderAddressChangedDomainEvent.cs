using GoldenTable.Common.Domain;
using GoldenTable.Modules.Order.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Order.Domain.Orders.Events;

public sealed class OrderAddressChangedDomainEvent(Guid orderId, Address newAddress) : DomainEvent
{
    public Guid OrderId { get; } =  orderId;
    public Address NewAddress { get; } = newAddress;
}
