using GoldenTable.Common.Application.EventBus;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;

namespace GoldenTable.Modules.Catalog.IntegrationEvents;

public sealed class DishUpdatedSizesIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid dishId,
    IReadOnlyCollection<DishSizeModel> sizes)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid DishId { get; } = dishId;
    public IReadOnlyCollection<DishSizeModel> Sizes { get; } = sizes;
}
