using GoldenTable.Common.Application.EventBus;

namespace GoldenTable.Modules.Catalog.IntegrationEvents;

public sealed class DishDeletedIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid dishId)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid DishId { get; } = dishId;
}
