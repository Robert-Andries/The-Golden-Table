using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedImagesDomainEvent(Guid dishId, IReadOnlyCollection<Guid> imageIds) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public IReadOnlyCollection<Guid> ImageIds { get; } = imageIds;
}
