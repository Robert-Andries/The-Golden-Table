using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedNameDomainEvent(Guid dishId, Name newName) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public Name NewName { get; } = newName;
}
