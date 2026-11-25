using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Dishes.Events;

public sealed class DishCreatedDomainEvent(Guid dishId) : DomainEvent
{
    public Guid DishId { get; } = dishId;
}
