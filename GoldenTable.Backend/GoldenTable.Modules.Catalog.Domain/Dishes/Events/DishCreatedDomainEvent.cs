using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishCreatedDomainEvent(Guid dishId) : DomainEvent
{
    public Guid DishId { get; init; } = dishId;
}
