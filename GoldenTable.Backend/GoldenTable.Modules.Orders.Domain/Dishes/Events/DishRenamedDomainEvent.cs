using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Dishes.Events;

public sealed class DishRenamedDomainEvent(Guid dishId, Name newName) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public Name NewName { get; } = newName;
}
