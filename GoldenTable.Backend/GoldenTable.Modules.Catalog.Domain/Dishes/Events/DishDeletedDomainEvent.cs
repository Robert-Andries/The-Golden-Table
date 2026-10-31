using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishDeletedDomainEvent(Guid dishId) : DomainEvent
{
    public Guid DishId { get; } = dishId;
}
