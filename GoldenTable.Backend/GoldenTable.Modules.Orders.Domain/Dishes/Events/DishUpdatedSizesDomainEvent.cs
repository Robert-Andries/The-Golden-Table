using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;

namespace GoldenTable.Modules.Orders.Domain.Dishes.Events;

public sealed class DishUpdatedSizesDomainEvent(Guid dishId, IReadOnlyCollection<DishSize> sizes) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public IReadOnlyCollection<DishSize> Sizes { get; } = sizes;
}
