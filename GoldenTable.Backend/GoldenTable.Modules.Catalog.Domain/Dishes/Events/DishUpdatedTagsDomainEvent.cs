using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedTagsDomainEvent(Guid dishId, IReadOnlyCollection<Guid> tagIds) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public IReadOnlyCollection<Guid> TagIds { get; } = tagIds;
}
