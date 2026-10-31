using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedSizesDomainEvent(Guid dishId, IReadOnlyCollection<DishSize> sizes) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public IReadOnlyCollection<DishSize> Sizes { get; } = sizes;
}
