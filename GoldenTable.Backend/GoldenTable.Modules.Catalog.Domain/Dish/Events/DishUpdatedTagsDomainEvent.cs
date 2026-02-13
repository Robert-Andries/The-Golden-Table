using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dish.Events;

public sealed record DishUpdatedTagsDomainEvent(Guid DishId, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
